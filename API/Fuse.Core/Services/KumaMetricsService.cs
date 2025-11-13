using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Fuse.Core.Interfaces;
using Fuse.Core.Responses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fuse.Core.Services;

public class KumaMetricsService : BackgroundService, IKumaHealthService
{
    private readonly ILogger<KumaMetricsService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFuseStore _store;
    private readonly ConcurrentDictionary<string, HealthStatusResponse> _healthCache = new();
    private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(1);

    public KumaMetricsService(
        ILogger<KumaMetricsService> logger,
        IHttpClientFactory httpClientFactory,
        IFuseStore store)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _store = store;
    }

    public HealthStatusResponse? GetHealthStatus(string monitorUrl)
    {
        if (string.IsNullOrWhiteSpace(monitorUrl))
            return null;

        // Normalize URL for comparison (remove trailing slashes, etc.)
        var normalizedUrl = NormalizeUrl(monitorUrl);
        _healthCache.TryGetValue(normalizedUrl, out var status);
        return status;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kuma Metrics Service starting...");

        // Wait a bit on startup to let other services initialize
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateMetricsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Kuma metrics");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }

        _logger.LogInformation("Kuma Metrics Service stopping.");
    }

    private async Task UpdateMetricsAsync(CancellationToken ct)
    {
        var snapshot = await _store.GetAsync(ct);
        var integrations = snapshot.KumaIntegrations;

        if (!integrations.Any())
        {
            _logger.LogDebug("No Kuma integrations configured");
            return;
        }

        _logger.LogDebug("Updating metrics for {Count} Kuma integration(s)", integrations.Count);

        foreach (var integration in integrations)
        {
            try
            {
                await FetchMetricsForIntegrationAsync(integration.Uri, integration.ApiKey, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch metrics from {Uri}", integration.Uri);
            }
        }
    }

    private async Task FetchMetricsForIntegrationAsync(Uri baseUri, string apiKey, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("kuma-metrics");
        
        try
        {
            var metricsUrl = new Uri(baseUri, "/metrics");
            var request = new HttpRequestMessage(HttpMethod.Get, metricsUrl);
            
            // Add authorization header if needed
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
            }

            using var response = await client.SendAsync(request, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch metrics from {Uri}: {StatusCode}", metricsUrl, response.StatusCode);
                return;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            ParseAndCacheMetrics(content);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception fetching metrics from {Uri}", baseUri);
        }
    }

    private void ParseAndCacheMetrics(string metricsContent)
    {
        // Parse Prometheus-format metrics
        // Example line: monitor_status{monitor_id="1",monitor_name="fuse",monitor_type="http",monitor_url="http://fuse.ubuntu.lan",monitor_hostname="null",monitor_port="null"} 1
        
        var regex = new Regex(
            @"monitor_status\{[^}]*monitor_url=""([^""]+)""[^}]*monitor_name=""([^""]+)""[^}]*\}\s+(\d+)",
            RegexOptions.Multiline | RegexOptions.IgnoreCase
        );

        var matches = regex.Matches(metricsContent);
        var now = DateTime.UtcNow;

        foreach (Match match in matches)
        {
            if (match.Groups.Count >= 4)
            {
                var monitorUrl = match.Groups[1].Value;
                var monitorName = match.Groups[2].Value;
                var statusValue = int.Parse(match.Groups[3].Value);

                var normalizedUrl = NormalizeUrl(monitorUrl);
                
                var status = statusValue switch
                {
                    1 => MonitorStatus.Up,
                    0 => MonitorStatus.Down,
                    2 => MonitorStatus.Pending,
                    3 => MonitorStatus.Maintenance,
                    _ => MonitorStatus.Down
                };

                var healthStatus = new HealthStatusResponse(
                    MonitorUrl: normalizedUrl,
                    Status: status,
                    MonitorName: monitorName,
                    LastChecked: now
                );

                _healthCache.AddOrUpdate(normalizedUrl, healthStatus, (_, _) => healthStatus);
                
                _logger.LogDebug("Cached health status for {Url}: {Status}", normalizedUrl, status);
            }
        }
    }

    private static string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        // Remove trailing slashes and normalize to lowercase for consistent comparison
        return url.TrimEnd('/').ToLowerInvariant();
    }
}

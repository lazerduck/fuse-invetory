using Fuse.Core.Interfaces;
using Fuse.Core.Manifests;
using Microsoft.AspNetCore.Mvc;

namespace Fuse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var data = await _serviceService.GetAllServiceManifestsAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(Guid id)
    {
        var data = await _serviceService.GetServiceManifestAsync(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] ServiceManifest manifest)
    {
        await _serviceService.CreateServiceManifestAsync(manifest);
        return CreatedAtAction(nameof(GetService), new { id = manifest.Id }, manifest);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] ServiceManifest manifest)
    {
        if (id != manifest.Id)
        {
            return BadRequest("ID in URL does not match ID in body.");
        }

        await _serviceService.UpdateServiceManifestAsync(manifest);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        await _serviceService.DeleteServiceManifestAsync(id);
        return NoContent();
    }
}

namespace Fuse.Core.Manifests
{
    public record Tags(
        Guid Id,
        string Name,
        string? Description,
        string? Color
    );
}
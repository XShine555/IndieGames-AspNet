namespace Application.Games.Catalog.Responses
{
    public record ApplicationGameReleaseBuild(
        Guid BuildId,
        string VersionName,
        string ManifestS3Path);
}

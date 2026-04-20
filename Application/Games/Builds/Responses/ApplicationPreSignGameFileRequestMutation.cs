namespace Application.Games.Builds.Responses
{
    public record ApplicationPreSignGameFileRequestMutation(
        string OriginalFilePath,
        string StorageKey,
        string UploadUrl);
}

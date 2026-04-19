namespace Application.Games.Responses
{
    public record ApplicationPreSignGameFileRequestMutation(
        string OriginalFilePath,
        string StorageKey,
        string UploadUrl);
}
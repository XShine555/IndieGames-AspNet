namespace Application.Games.Responses
{
    public record ApplicationUploadGameFilesRequestMutation(
        string StorageKey,
        string UploadUrl);
}
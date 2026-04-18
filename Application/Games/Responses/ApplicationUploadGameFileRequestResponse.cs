namespace Application.Games.Responses
{
    public record ApplicationUploadGameFileRequestResponse(
        string StorageKey,
        string UploadUrl);
}
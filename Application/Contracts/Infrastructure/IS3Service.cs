namespace Application.Contracts.Infrastructure
{
    public interface IS3Service
    {
        Task UploadFileAsync(Stream fileStream, string keyName, string contentType, CancellationToken cancellationToken);
        Task RemoveFileAsync(string keyName, CancellationToken cancellationToken);
        Task<Stream> GetFileStreamAsync(string keyName, CancellationToken cancellationToken);
        Task<string> GetSignedUrlAsync(string keyName, TimeSpan expiration, CancellationToken cancellationToken);
    }
}
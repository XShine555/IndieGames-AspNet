using Application.Abstractions.Common;

namespace Application.Abstractions.Storage
{
    public interface IS3Service
    {
        Task UploadFileAsync(IFileData fileData, string keyName, CancellationToken cancellationToken);
        Task UploadFileAsync(Stream fileStream, string keyName, string contentType, CancellationToken cancellationToken);
        Task RemoveFileAsync(string keyName, CancellationToken cancellationToken);
        Task<Stream> GetFileStreamAsync(string keyName, CancellationToken cancellationToken);
        Task<string> GetSignedUrlAsync(string keyName, TimeSpan expiration, CancellationToken cancellationToken);
    }
}

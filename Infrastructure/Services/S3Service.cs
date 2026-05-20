using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Common;
using Application.Abstractions.Storage;
using Infrastructure.Configurations;
using System.Net;

namespace Infrastructure.Services
{
    public class S3Service(IAmazonS3 amazonS3, S3Configuration s3Configuration)
        : IS3Service
    {
        public async Task UploadFileAsync(Stream fileStream, string keyName, string contentType, CancellationToken cancellationToken)
        {
            // ReferenceReadStream (IFormFile) reports CanSeek=true but throws when the AWS SDK
            // seeks back to position 0 to compute content-length. Copy to MemoryStream first.
            MemoryStream buffer = new();
            await fileStream.CopyToAsync(buffer, cancellationToken);
            buffer.Position = 0;

            var request = new PutObjectRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
                InputStream = buffer,
                ContentType = contentType,
            };
            await amazonS3.PutObjectAsync(request, cancellationToken);
        }

        public Task RemoveFileAsync(string keyName, CancellationToken cancellationToken)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
            };
            return amazonS3.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task<Stream> GetFileStreamAsync(string keyName, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
            };
            var result = await amazonS3.GetObjectAsync(request, cancellationToken);
            return result.ResponseStream;
        }

        public async Task<string> GetSignedUrlAsync(string keyName, TimeSpan expiration, CancellationToken cancellationToken)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
                Expires = DateTime.UtcNow + expiration,
            };
            var result = await amazonS3.GetPreSignedURLAsync(request);
            return result;
        }

        public async Task<string> GetUploadUrlAsync(string keyName, TimeSpan expiration, CancellationToken cancellationToken)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow + expiration,
            };
            var result = await amazonS3.GetPreSignedURLAsync(request);
            return result;
        }

        public Task UploadFileAsync(IFileData fileData, string keyName, CancellationToken cancellationToken)
        {
            return UploadFileAsync(fileData.FileStream, keyName, fileData.ContentType, cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetFileListAsync(string route, CancellationToken cancellationToken)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = s3Configuration.BucketName,
                Prefix = route,
            };
            var result = await amazonS3.ListObjectsV2Async(request, cancellationToken);
            return result.S3Objects?.Select(o => o.Key).ToList() ?? [];
        }

        public async Task<bool> FileExistsAsync(string keyName, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = s3Configuration.BucketName,
                    Key = keyName,
                };
                var result = await amazonS3.GetObjectMetadataAsync(request, cancellationToken);
            }
            catch (AmazonS3Exception amazonS3Exception)
                when (amazonS3Exception.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            return true;
        }

        public async Task<S3FileData> GetFileDataAsync(string keyName, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
            };
            var result = await amazonS3.GetObjectAsync(request, cancellationToken);
            return new S3FileData(
                Path.GetDirectoryName(result.Key)?.Replace("\\", "/"),
                Path.GetFileName(result.Key),
                result.Headers.ContentType,
                result.ContentLength
            );
        }
    }
}
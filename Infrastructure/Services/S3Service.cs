using Amazon.S3;
using Amazon.S3.Model;
using Application.Contracts.Infrastructure;
using Infrastructure.Configurations;

namespace Infrastructure.Services
{
    public class S3Service(IAmazonS3 amazonS3, S3Configuration s3Configuration)
        : IS3Service
    {
        public Task UploadFileAsync(Stream fileStream, string keyName, string contentType, CancellationToken cancellationToken)
        {
            var request = new PutObjectRequest
            {
                BucketName = s3Configuration.BucketName,
                Key = keyName,
                InputStream = fileStream,
                ContentType = contentType,
            };
            return amazonS3.PutObjectAsync(request, cancellationToken);
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
            var result = amazonS3.GetPreSignedURL(request);
            return result;
        }
    }
}
using SixLabors.ImageSharp;

namespace Application.Contracts.Infrastructure
{
    public interface IPictureService
    {
        Task<Stream> ResizePictureAsWebpAsync(Stream pictureStream, Size size, CancellationToken cancellationToken);
    }
}
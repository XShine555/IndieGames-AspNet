using SixLabors.ImageSharp;

namespace Application.Abstractions
{
    public interface IPictureService
    {
        Task<Stream> ResizePictureAsWebpAsync(Stream pictureStream, Size size, CancellationToken cancellationToken);
    }
}

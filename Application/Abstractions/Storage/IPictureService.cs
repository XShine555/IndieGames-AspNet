using SixLabors.ImageSharp;

namespace Application.Abstractions.Storage
{
    public interface IPictureService
    {
        Task<Stream> ResizePictureAsWebpAsync(Stream pictureStream, Size size, CancellationToken cancellationToken);
        Task<bool> IsValidImageFormatAsync(Stream pictureStream, CancellationToken cancellationToken);
    }
}

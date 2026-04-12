using Application.Abstractions.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Services
{
    public class PictureService : IPictureService
    {
        public async Task<Stream> ResizePictureAsWebpAsync(Stream pictureStream, Size size, CancellationToken cancellationToken)
        {
            if (pictureStream.CanSeek && pictureStream.Position > 0)
                pictureStream.Position = 0;

            using var picture = await Image.LoadAsync(pictureStream, cancellationToken);
            picture.Mutate(options => options.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Max
            } ));

            var memoryStream = new MemoryStream();
            await picture.SaveAsWebpAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
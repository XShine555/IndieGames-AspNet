using System.Text.Json.Serialization;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record UploadFileToBucketArguments(
        string FilePathVariable,
        string DestinationRoute,
        [property: JsonIgnore]
        Func<CancellationToken, Task>? OnError = null);
}

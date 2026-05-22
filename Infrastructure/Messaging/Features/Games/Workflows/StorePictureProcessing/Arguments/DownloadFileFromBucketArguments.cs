using System.Text.Json.Serialization;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record DownloadFileFromBucketArguments(
        string Key,
        string DestinationFilePathVariable,
        [property: JsonIgnore]
        Func<CancellationToken, Task>? OnError = null);
}

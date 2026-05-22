using System.Text.Json.Serialization;
using Infrastructure.Messaging.Features.Common.Workflows;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record DownloadFileFromBucketArguments(
        string Key,
        string DestinationFilePathVariable,
        PictureProcessingWorkflowContextType WorkflowContextType = PictureProcessingWorkflowContextType.None,
        Guid? EntityId = null);
}

using Infrastructure.Messaging.Features.Common.Workflows;

namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record UploadFileToBucketArguments(
        string FilePathVariable,
        string DestinationRoute,
        PictureProcessingWorkflowContextType WorkflowContextType = PictureProcessingWorkflowContextType.None,
        Guid? EntityId = null);
}

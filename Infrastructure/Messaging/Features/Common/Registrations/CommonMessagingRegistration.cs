using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs;
using MassTransit;

namespace Infrastructure.Messaging.Features.Common.Registrations
{
    internal static class CommonMessagingRegistration
    {
        internal static void AddCommonMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddActivity<DownloadFileFromBucketActivity, DownloadFileFromBucketArguments, DownloadFileFromBucketLog>();
            options.AddActivity<ResizePictureActivity, ResizePictureLocalArguments, ResizePictureLog>();
            options.AddActivity<UploadFileToBucketActivity, UploadFileToBucketArguments, UploadFileToBucketLog>();
            options.AddExecuteActivity<GeneratePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>();
        }
    }
}
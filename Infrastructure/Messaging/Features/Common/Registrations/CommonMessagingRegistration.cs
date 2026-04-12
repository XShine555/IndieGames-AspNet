using Infrastructure.Messaging.Features.Common.Activities.Files;
using Infrastructure.Messaging.Features.Common.Activities.Pictures;
using Infrastructure.Messaging.Features.Games.Activities;
using Infrastructure.Messaging.Features.Games.Consumers;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments;
using Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Logs;
using MassTransit;

namespace Infrastructure.Messaging.Features.Common.Registrations
{
    internal static class CommonMessagingRegistration
    {
        internal static void AddCommonMessaging(this IBusRegistrationConfigurator options)
        {
            options.AddConsumer<GenerateGamesPicturesConsumer>();
            options.AddExecuteActivity<GeneratePictureWorkflowPathsActivity, GeneratePictureWorkflowPathsArguments>();
            options.AddActivity<DownloadFileFromBucketActivity, DownloadFileFromBucketArguments, DownloadFileFromBucketLog>();
            options.AddActivity<ResizePictureActivity, ResizePictureLocalArguments, ResizePictureLog>();
            options.AddActivity<UploadFileToBucketActivity, UploadFileToBucketArguments, UploadFileToBucketLog>();
        }
    }
}
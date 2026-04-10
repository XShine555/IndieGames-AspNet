namespace Infrastructure.MassTransit.RoutingSlip.Variables
{
    internal static class RoutingSlipVariableNames
    {
        internal static class Workflow
        {
            internal const string TemporalDirectory = "Workflow.TemporalDirectory";
            internal const string CorrelationId = "Workflow.CorrelationId";
        }

        internal static class Picture
        {
            internal const string OriginalFilePath = "Picture.OriginalFilePath";
            internal const string SmallResizedFilePath = "Picture.SmallResizedFilePath";
            internal const string MediumResizedFilePath = "Picture.MediumResizedFilePath";
            internal const string LargeResizedFilePath = "Picture.LargeResizedFilePath";
            internal const string DestinationFolderName = "Picture.DestinationFolderName";
        }
    }
}
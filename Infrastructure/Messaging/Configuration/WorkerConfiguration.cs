namespace Infrastructure.Messaging.Configuration
{
    public class WorkerConfiguration
    {
        public const string SectionName = "Worker";

        public WorkerRoutes Routes { get; set; } = new();
    }

    public class WorkerRoutes
    {
        public string TemporaryFilesDirectory { get; set; } = Path.GetTempPath();
    }
}

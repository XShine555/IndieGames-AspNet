using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.JobTracking
{
#pragma warning disable CS8618
    [Table("Job_Tracking")]
    public class JobTracking
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? CorrelationId { get; set; }

        public Guid? ConversationId { get; set; }

        public Guid? MessageId { get; set; }

        [Required]
        [MaxLength(256)]
        public required string JobName { get; set; }

        [Required]
        public JobTrackingStatus Status { get; set; } = JobTrackingStatus.Running;

        [Required]
        public DateTime StartedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime? FinishedDateTime { get; set; }

        [MaxLength(2048)]
        public string? ErrorMessage { get; set; }

        public ICollection<JobTrackingStep> Steps { get; set; } = new List<JobTrackingStep>();
    }
}
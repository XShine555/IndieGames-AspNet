using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.JobTracking
{
#pragma warning disable CS8618
    [Table("Job_Tracking_Step")]
    public class JobTrackingStep
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid JobTrackingId { get; set; }

        [Required]
        [MaxLength(256)]
        public required string StepName { get; set; }

        [Required]
        public JobTrackingType ComponentType { get; set; }

        [Required]
        public JobTrackingStatus Status { get; set; } = JobTrackingStatus.Running;

        [Required]
        public int Attempt { get; set; }

        [Required]
        public DateTime StartedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime? FinishedDateTime { get; set; }

        [MaxLength(2048)]
        public string? ErrorMessage { get; set; }

        [ForeignKey(nameof(JobTrackingId))]
        public JobTracking JobTracking { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ProcessExecutions
{
#pragma warning disable CS8618
    [Table("Process_Step_Execution")]
    public class ProcessStepExecution
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ProcessExecutionId { get; set; }

        [Required]
        [MaxLength(256)]
        public required string StepName { get; set; }

        [Required]
        public ProcessStepComponentType ComponentType { get; set; }

        [Required]
        public ProcessExecutionStatus Status { get; set; } = ProcessExecutionStatus.Running;

        [Required]
        public int Attempt { get; set; }

        [Required]
        public DateTime StartedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime? FinishedDateTime { get; set; }

        [MaxLength(2048)]
        public string? ErrorMessage { get; set; }

        [ForeignKey(nameof(ProcessExecutionId)) ]
        public ProcessExecution ProcessExecution { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
#pragma warning disable CS8618
    [Table("User_Profile_Pictures")]
    public class UserProfilePictures
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [MaxLength(128)]
        public string? OriginalName { get; set; }

        [MaxLength(128)]
        public string? OriginalRelativePath { get; set; }

        [MaxLength(16)]
        public string? OriginalFileExtension { get; set; }

        [Required]
        [MaxLength(128)]
        public required string SmallRelativePath { get; set; }

        [Required]
        [MaxLength(36)]
        public required string SmallName { get; set; }

        [Required]
        [MaxLength(16)]
        public required string SmallFileExtension { get; set; }

        [Required]
        [MaxLength(128)]
        public required string MediumRelativePath { get; set; }

        [Required]
        [MaxLength(36)]
        public required string MediumName { get; set; }

        [Required]
        [MaxLength(16)]
        public required string MediumFileExtension { get; set; }

        [Required]
        [MaxLength(128)]
        public required string LargeRelativePath { get; set; }

        [Required]
        [MaxLength(36)]
        public required string LargeName { get; set; }

        [Required]
        [MaxLength(16)]
        public required string LargeFileExtension { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId)) ]
        public User User { get; set; }
    }
}

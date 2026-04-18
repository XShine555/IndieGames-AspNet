using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Games.Entities
{
#pragma warning disable CS8618
    [Table("Game_Build_Files")]
    public class GameBuildFile
    {
        [Key]
        public Guid Id { get; set; }

        public Guid GameBuildId { get; set; }

        [Required]
        public required string FileRelativePath { get; set; }

        [Required]
        public required string FileName { get; set; }

        [Required]
        public required string FileContentType { get; set; }

        public long FileSize { get; set; }

        [Required]
        public required string Hash { get; set; }

        [Required]
        public required string HashAlgorithm { get; set; }

        [ForeignKey(nameof(GameBuildId) )]
        public GameBuild GameBuild { get; set; }
    }
}

using Domain.Entities;
using Domain.Games.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Games.Entities
{
#pragma warning disable CS8618
    [Table("Game_Builds")]
    public class GameBuild
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required Guid GameId { get; set; }

        [Required]
        public required string VersionName { get; set; }

        public string? ManifestRelativePath { get; set; }

        public string? ManifestFileName { get; set; }

        public string? ManifestContentType { get; set; }

        public string? ExecutableRelativePath { get; set; }

        public string? ExecutableFileName { get; set; }

        public string? ExecutableContentType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public GameBuildStatus Status { get; set; } = GameBuildStatus.UploadingFiles;

        [ForeignKey(nameof(GameId)) ]
        public Game Game { get; set; }

        public ICollection<GameBuildFile> Files { get; set; } = new List<GameBuildFile>();
    }
}
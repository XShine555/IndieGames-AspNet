using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Games.Entities
{
#pragma warning disable CS8618
    public class GameBuild
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }

        [Required]
        public required string VersionName { get; set; }

        [Required]
        public required string StorageKey { get; set; }

        public long FileSize { get; set; }

        [Required]
        public required string Hash { get; set; }

        [Required]
        public required string HashAlgorithm { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(GameId)) ]
        public Game Game { get; set; }
    }
}
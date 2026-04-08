using Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Genres")]
    public class Genre : IUpdatableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(32)]
        public required string NormalizedName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
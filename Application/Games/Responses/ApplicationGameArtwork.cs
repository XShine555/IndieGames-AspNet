using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGameArtwork(
        Guid ArtworkId,
        GameArtworkType Type,
        string OriginalArtworkKey,
        string? SmallArtworkKey,
        string? MediumArtworkKey,
        string? LargeArtworkKey,
        GameArtworkProcessingStatus ProcessingStatus,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}

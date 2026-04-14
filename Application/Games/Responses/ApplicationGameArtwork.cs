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
        DateTime AddedAt)
    {
        public static ApplicationGameArtwork FromEntity(GameArtwork artwork)
        {
            return new ApplicationGameArtwork(
                artwork.Id,
                artwork.Type,
                BuildArtworkKey(artwork.OriginalRelativePath, artwork.OriginalFileName),
                BuildArtworkKey(artwork.SmallRelativePath, artwork.SmallFileName),
                BuildArtworkKey(artwork.MediumRelativePath, artwork.MediumFileName),
                BuildArtworkKey(artwork.LargeRelativePath, artwork.LargeFileName),
                artwork.ProcessingStatus,
                artwork.CreatedAt);
        }

        private static string BuildArtworkKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;
            return $"{relativePath}/{name}";
        }
    }
}

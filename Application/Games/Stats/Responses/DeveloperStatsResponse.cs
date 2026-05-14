namespace Application.Games.Stats.Responses
{
    public record DeveloperStatsResponse(
        int GamesSold,
        string GamesSoldSubtitle,
        int Players,
        string PlayersSubtitle,
        int PublishedGames,
        string PublishedGamesSubtitle,
        int GamesWithIssues,
        string GamesWithIssuesSubtitle);
}

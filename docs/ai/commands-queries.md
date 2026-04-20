# Commands & Queries Catalog

Este archivo se genera automaticamente desde `Application` para que GitHub Copilot u otra IA pueda descubrir rapidamente los `Commands` y `Queries` disponibles.

> Para actualizarlo: `powershell -ExecutionPolicy Bypass -File tools\\Update-CommandsQueriesCatalog.ps1`

## Games

| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |
| --- | --- | --- | --- | --- | --- |
| Command | `AddStorePictureToGameCommand` | `Application.Games.Media.Commands` | `Result<ApplicationGamePicture>` | `Application/Games/Media/Handlers/AddStorePictureToGameCommandHandler.cs` | `Application/Games/Media/Commands/AddStorePictureToGameCommand.cs` |
| Command | `ChangeGameOwnerCommand` | `Application.Games.Catalog.Commands` | `Result<ApplicationGameMutation>` | `Application/Games/Catalog/Handlers/ChangeGameOwnerCommandHandler.cs` | `Application/Games/Catalog/Commands/ChangeGameOwnerCommand.cs` |
| Command | `CompleteGameBuildCommand` | `Application.Games.Builds.Commands` | `Result` | `Application/Games/Builds/Handlers/CompleteGameBuildCommandHandler.cs` | `Application/Games/Builds/Commands/CompleteGameBuildCommand.cs` |
| Command | `CreateGameBuildCommand` | `Application.Games.Builds.Commands` | `Result<ApplicationGameBuildMutation>` | `Application/Games/Builds/Handlers/CreateGameBuildCommandHandler.cs` | `Application/Games/Builds/Commands/CreateGameBuildCommand.cs` |
| Command | `CreateGameCommand` | `Application.Games.Catalog.Commands` | `Result<ApplicationGameMutation>` | `Application/Games/Catalog/Handlers/CreateGameCommandHandler.cs` | `Application/Games/Catalog/Commands/CreateGameCommand.cs` |
| Command | `PreSignGameFilesRequestCommand` | `Application.Games.Builds.Commands` | `Result<IReadOnlyList<ApplicationPreSignGameFileRequestMutation>>` | `Application/Games/Builds/Handlers/PreSignGameFilesRequestCommandHandler.cs` | `Application/Games/Builds/Commands/PreSignGameFilesRequestCommand.cs` |
| Command | `PublishGameCommand` | `Application.Games.Catalog.Commands` | `Result` | `Application/Games/Catalog/Handlers/PublishGameCommandHandler.cs` | `Application/Games/Catalog/Commands/PublishGameCommand.cs` |
| Command | `RemoveGameBuildCommand` | `Application.Games.Builds.Commands` | `Result` | `Application/Games/Builds/Handlers/RemoveGameBuildCommandHandler.cs` | `Application/Games/Builds/Commands/RemoveGameBuildCommand.cs` |
| Command | `RemoveGameCommand` | `Application.Games.Catalog.Commands` | `Result` | `Application/Games/Catalog/Handlers/RemoveGameCommandHandler.cs` | `Application/Games/Catalog/Commands/RemoveGameCommand.cs` |
| Command | `RemoveStorePictureToGameCommand` | `Application.Games.Media.Commands` | `Result` | `N/A` | `Application/Games/Media/Commands/RemoveStorePictureToGameCommand.cs` |
| Command | `RetryGameArtworkCommand` | `Application.Games.Media.Commands` | `Result` | `Application/Games/Media/Handlers/RetryGameArtworkCommandHandler.cs` | `Application/Games/Media/Commands/RetryGameArtworkCommand.cs` |
| Command | `RetryGameStorePictureCommand` | `Application.Games.Media.Commands` | `Result` | `Application/Games/Media/Handlers/RetryGameStorePictureCommandHandler.cs` | `Application/Games/Media/Commands/RetryGameStorePictureCommand.cs` |
| Command | `UpdateGameArtworkCommand` | `Application.Games.Media.Commands` | `Result<ApplicationGameArtwork>` | `Application/Games/Media/Handlers/UpdateGameArtworkCommandHandler.cs` | `Application/Games/Media/Commands/UpdateGameArtworkCommand.cs` |
| Command | `UpdateGameBuildCommand` | `Application.Games.Builds.Commands` | `Result<ApplicationGameBuildMutation>` | `Application/Games/Builds/Handlers/UpdateGameBuildCommandHandler.cs` | `Application/Games/Builds/Commands/UpdateGameBuildCommand.cs` |
| Command | `UpdateGameCommand` | `Application.Games.Catalog.Commands` | `Result<ApplicationGameMutation>` | `Application/Games/Catalog/Handlers/UpdateGameCommandHandler.cs` | `Application/Games/Catalog/Commands/UpdateGameCommand.cs` |
| Command | `UpdateGameGenresCommand` | `Application.Games.Catalog.Commands` | `Result<ApplicationGameGenresMutation>` | `Application/Games/Catalog/Handlers/UpdateGameGenresCommandHandler.cs` | `Application/Games/Catalog/Commands/UpdateGameGenresCommand.cs` |
| Query | `GetGameBuildByIdQuery` | `Application.Games.Builds.Queries` | `Result<ApplicationGameBuild>` | `Application/Games/Builds/Handlers/GetGameBuildByIdQueryHandler.cs` | `Application/Games/Builds/Queries/GetGameBuildByIdQuery.cs` |
| Query | `GetGameBuildsQuery` | `Application.Games.Builds.Queries` | `Result<IReadOnlyCollection<ApplicationGameBuild>>` | `Application/Games/Builds/Handlers/GetGameBuildsQueryHandler.cs` | `Application/Games/Builds/Queries/GetGameBuildsQuery.cs` |
| Query | `GetGameByIdQuery` | `Application.Games.Catalog.Queries` | `Result<ApplicationGame>` | `Application/Games/Catalog/Handlers/GetGameByIdQueryHandler.cs` | `Application/Games/Catalog/Queries/GetGameByIdQuery.cs` |
| Query | `GetGamesQuery` | `Application.Games.Catalog.Queries` | `PaginatedApplicationResponse<ApplicationGameListItem>` | `Application/Games/Catalog/Handlers/GetGamesQueryHandler.cs` | `Application/Games/Catalog/Queries/GetGamesQuery.cs` |

## Genres

| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |
| --- | --- | --- | --- | --- | --- |
| Command | `CreateGenreCommand` | `Application.Genres.Commands` | `Result<ApplicationGenreMutation>` | `Application/Genres/Handlers/CreateGenreCommandHandler.cs` | `Application/Genres/Commands/CreateGenreCommand.cs` |
| Command | `RemoveGenreCommand` | `Application.Genres.Commands` | `Result` | `Application/Genres/Handlers/RemoveGenreCommandHandler.cs` | `Application/Genres/Commands/RemoveGenreCommand.cs` |
| Command | `UpdateGenreCommand` | `Application.Genres.Commands` | `Result<ApplicationGenreMutation>` | `Application/Genres/Handlers/UpdateGenreCommandHandler.cs` | `Application/Genres/Commands/UpdateGenreCommand.cs` |
| Query | `GetGenreByIdQuery` | `Application.Genres.Queries` | `Result<ApplicationGenre>` | `Application/Genres/Handlers/GetGenreByIdQueryHandler.cs` | `Application/Genres/Queries/GetGenreByIdQuery.cs` |
| Query | `GetGenresQuery` | `Application.Genres.Queries` | `PaginatedApplicationResponse<ApplicationGenreListItem>` | `Application/Genres/Handlers/GetGenresQueryHandler.cs` | `Application/Genres/Queries/GetGenresQuery.cs` |

## Users

| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |
| --- | --- | --- | --- | --- | --- |
| Command | `AddGameToUserCartCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/AddGameToUserCartCommandHandler.cs` | `Application/Users/Commands/AddGameToUserCartCommand.cs` |
| Command | `AddGameToUserCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/AddGameToUserCollectionCommandHandler.cs` | `Application/Users/Commands/AddGameToUserCollectionCommand.cs` |
| Command | `AddGameToUserLibraryCommand` | `Application.Users.Commands` | `Result<ApplicationUserOwnedGame>` | `Application/Users/Handlers/AddGameToUserLibraryCommandHandler.cs` | `Application/Users/Commands/AddGameToUserLibraryCommand.cs` |
| Command | `CreateUserCommand` | `Application.Users.Commands` | `Result<ApplicationUserMutation>` | `Application/Users/Handlers/CreateUserCommandHandler.cs` | `Application/Users/Commands/CreateUserCommand.cs` |
| Command | `CreateUserGameCollectionCommand` | `Application.Users.Commands` | `Result<ApplicationUserCollectionListItem>` | `Application/Users/Handlers/CreateUserGameCollectionCommandHandler.cs` | `Application/Users/Commands/CreateUserGameCollectionCommand.cs` |
| Command | `PromoteUserToDeveloperCommand` | `Application.Users.Commands` | `Result<ApplicationUserMutation>` | `Application/Users/Handlers/PromoteUserToDeveloperCommandHandler.cs` | `Application/Users/Commands/PromoteUserToDeveloperCommand.cs` |
| Command | `RemoveGameFromUserCartCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/RemoveGameFromUserCartCommandHandler.cs` | `Application/Users/Commands/RemoveGameFromUserCartCommand.cs` |
| Command | `RemoveGameFromUserCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/RemoveGameFromUserCollectionCommandHandler.cs` | `Application/Users/Commands/RemoveGameFromUserCollectionCommand.cs` |
| Command | `RemoveGameToUserCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/RemoveGameToUserCommandHandler.cs` | `Application/Users/Commands/RemoveGameToUserCommand.cs` |
| Command | `RemoveUserGameCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/RemoveUserGameCollectionCommandHandler.cs` | `Application/Users/Commands/RemoveUserGameCollectionCommand.cs` |
| Command | `UpdateUserCommand` | `Application.Users.Commands` | `Result<ApplicationUserMutation>` | `Application/Users/Handlers/UpdateUserCommandHandler.cs` | `Application/Users/Commands/UpdateUserCommand.cs` |
| Command | `UpdateUserGameCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/UpdateUserGameCollectionCommandHandler.cs` | `Application/Users/Commands/UpdateUserGameCollectionCommand.cs` |
| Command | `UpdateUserProfilePictureCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/UpdateUserProfilePictureCommandHandler.cs` | `Application/Users/Commands/UpdateUserProfilePictureCommand.cs` |
| Query | `GetUserByIdentityIdQuery` | `Application.Users.Queries` | `Result<ApplicationUser>` | `Application/Users/Handlers/GetUserByIdentityIdQueryHandler.cs` | `Application/Users/Queries/GetUserByIdentityIdQuery.cs` |
| Query | `GetUserCartItemsQuery` | `Application.Users.Queries` | `Result<IReadOnlyCollection<ApplicationUserCartItem>>` | `Application/Users/Handlers/GetUserCartItemsQueryHandler.cs` | `Application/Users/Queries/GetUserCartItemsQuery.cs` |
| Query | `GetUserCollectionByIdQuery` | `Application.Users.Queries` | `Result<ApplicationUserCollectionDetails>` | `Application/Users/Handlers/GetUserCollectionByIdQueryHandler.cs` | `Application/Users/Queries/GetUserCollectionByIdQuery.cs` |
| Query | `GetUserCollectionsQuery` | `Application.Users.Queries` | `PaginatedApplicationResponse<ApplicationUserCollectionListItem>` | `Application/Users/Handlers/GetUserCollectionsQueryHandler.cs` | `Application/Users/Queries/GetUserCollectionsQuery.cs` |
| Query | `GetUsersQuery` | `Application.Users.Queries` | `PaginatedApplicationResponse<ApplicationUserListItem>` | `Application/Users/Handlers/GetUsersQueryHandler.cs` | `Application/Users/Queries/GetUsersQuery.cs` |



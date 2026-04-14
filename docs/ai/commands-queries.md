# Commands & Queries Catalog

Este archivo se genera automaticamente desde `Application` para que GitHub Copilot u otra IA pueda descubrir rapidamente los `Commands` y `Queries` disponibles.

> Para actualizarlo: `powershell -ExecutionPolicy Bypass -File tools\\Update-CommandsQueriesCatalog.ps1`

## Games

| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |
| --- | --- | --- | --- | --- | --- |
| Command | `AddStorePictureToGameCommand` | `Application.Games.Commands` | `Result<ApplicationGamePicture>` | `Application/Games/Handlers/AddStorePictureToGameCommandHandler.cs` | `Application/Games/Commands/AddStorePictureToGameCommand.cs` |
| Command | `ChangeGameOwnerCommand` | `Application.Games.Commands` | `Result<ApplicationGame>` | `Application/Games/Handlers/ChangeGameOwnerCommandHandler.cs` | `Application/Games/Commands/ChangeGameOwnerCommand.cs` |
| Command | `CreateGameCommand` | `Application.Games.Commands` | `Result<ApplicationGame>` | `Application/Games/Handlers/CreateGameCommandHandler.cs` | `Application/Games/Commands/CreateGameCommand.cs` |
| Command | `PublishGameCommand` | `Application.Games.Commands` | `Result<ApplicationGame>` | `Application/Games/Handlers/PublishGameCommandHandler.cs` | `Application/Games/Commands/PublishGameCommand.cs` |
| Command | `RemoveGameCommand` | `Application.Games.Commands` | `Result` | `Application/Games/Handlers/RemoveGameCommandHandler.cs` | `Application/Games/Commands/RemoveGameCommand.cs` |
| Command | `RemoveStorePictureToGameCommand` | `Application.Games.Commands` | `Result` | `N/A` | `Application/Games/Commands/RemoveStorePictureToGameCommand.cs` |
| Command | `RetryGameArtworkCommand` | `Application.Games.Commands` | `Result` | `Application/Games/Handlers/RetryGameArtworkCommandHandler.cs` | `Application/Games/Commands/RetryGameArtworkCommand.cs` |
| Command | `RetryGameStorePictureCommand` | `Application.Games.Commands` | `Result` | `Application/Games/Handlers/RetryGameStorePictureCommandHandler.cs` | `Application/Games/Commands/RetryGameStorePictureCommand.cs` |
| Command | `UpdateGameArtworkCommand` | `Application.Games.Commands` | `Result<ApplicationGameArtwork>` | `Application/Games/Handlers/UpdateGameArtworkCommandHandler.cs` | `Application/Games/Commands/UpdateGameArtworkCommand.cs` |
| Command | `UpdateGameCommand` | `Application.Games.Commands` | `Result<ApplicationGame>` | `Application/Games/Handlers/UpdateGameCommandHandler.cs` | `Application/Games/Commands/UpdateGameCommand.cs` |
| Command | `UpdateGameGenresCommand` | `Application.Games.Commands` | `Result<ApplicationGame>` | `Application/Games/Handlers/UpdateGameGenresCommandHandler.cs` | `Application/Games/Commands/UpdateGameGenresCommand.cs` |
| Query | `GetGameByIdQuery` | `Application.Games.Queries` | `Result<ApplicationGame>` | `Application/Games/Handlers/GetGameByIdQueryHandler.cs` | `Application/Games/Queries/GetGameByIdQuery.cs` |
| Query | `GetGamesQuery` | `Application.Games.Queries` | `PaginatedApplicationResponse<ApplicationGame>` | `Application/Games/Handlers/GetGamesQueryHandler.cs` | `Application/Games/Queries/GetGamesQuery.cs` |

## Genres

| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |
| --- | --- | --- | --- | --- | --- |
| Command | `CreateGenreCommand` | `Application.Genres.Commands` | `Result<ApplicationGenre>` | `Application/Genres/Handlers/CreateGenreCommandHandler.cs` | `Application/Genres/Commands/CreateGenreCommand.cs` |
| Command | `RemoveGenreCommand` | `Application.Genres.Commands` | `Result` | `Application/Genres/Handlers/RemoveGenreCommandHandler.cs` | `Application/Genres/Commands/RemoveGenreCommand.cs` |
| Command | `UpdateGenreCommand` | `Application.Genres.Commands` | `Result<ApplicationGenre>` | `Application/Genres/Handlers/UpdateGenreCommandHandler.cs` | `Application/Genres/Commands/UpdateGenreCommand.cs` |
| Query | `GetGenreByIdQuery` | `Application.Genres.Queries` | `Result<ApplicationGenre>` | `Application/Genres/Handlers/GetGenreByIdQueryHandler.cs` | `Application/Genres/Queries/GetGenreByIdQuery.cs` |
| Query | `GetGenresQuery` | `Application.Genres.Queries` | `PaginatedApplicationResponse<ApplicationGenre>` | `Application/Genres/Handlers/GetGenresQueryHandler.cs` | `Application/Genres/Queries/GetGenresQuery.cs` |

## Users

| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |
| --- | --- | --- | --- | --- | --- |
| Command | `AddGameToUserCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/AddGameToUserCollectionCommandHandler.cs` | `Application/Users/Commands/AddGameToUserCollectionCommand.cs` |
| Command | `AddGameToUserCommand` | `Application.Users.Commands` | `Result<ApplicationUser>` | `Application/Users/Handlers/AddGameToUserCommandHandler.cs` | `Application/Users/Commands/AddGameToUserCommand.cs` |
| Command | `AddGameToUserLibraryCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/AddGameToUserLibraryCommandHandler.cs` | `Application/Users/Commands/AddGameToUserLibraryCommand.cs` |
| Command | `CreateUserCommand` | `Application.Users.Commands` | `Result<ApplicationUser>` | `Application/Users/Handlers/CreateUserCommandHandler.cs` | `Application/Users/Commands/CreateUserCommand.cs` |
| Command | `CreateUserGameCollectionCommand` | `Application.Users.Commands` | `Result<Guid>` | `Application/Users/Handlers/CreateUserGameCollectionCommandHandler.cs` | `Application/Users/Commands/CreateUserGameCollectionCommand.cs` |
| Command | `DeleteUserGameCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/DeleteUserGameCollectionCommandHandler.cs` | `Application/Users/Commands/DeleteUserGameCollectionCommand.cs` |
| Command | `RemoveGameFromUserCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/RemoveGameFromUserCollectionCommandHandler.cs` | `Application/Users/Commands/RemoveGameFromUserCollectionCommand.cs` |
| Command | `RemoveGameToUserCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/RemoveGameToUserCommandHandler.cs` | `Application/Users/Commands/RemoveGameToUserCommand.cs` |
| Command | `UpdateUserCommand` | `Application.Users.Commands` | `Result<ApplicationUser>` | `Application/Users/Handlers/UpdateUserCommandHandler.cs` | `Application/Users/Commands/UpdateUserCommand.cs` |
| Command | `UpdateUserGameCollectionCommand` | `Application.Users.Commands` | `Result` | `Application/Users/Handlers/UpdateUserGameCollectionCommandHandler.cs` | `Application/Users/Commands/UpdateUserGameCollectionCommand.cs` |
| Command | `UpdateUserProfilePictureCommand` | `Application.Users.Commands` | `Result<ApplicationUser>` | `Application/Users/Handlers/UpdateUserProfilePictureCommandHandler.cs` | `Application/Users/Commands/UpdateUserProfilePictureCommand.cs` |
| Query | `GetUserByIdentityIdQuery` | `Application.Users.Queries` | `Result<ApplicationUser>` | `Application/Users/Handlers/GetUserByIdentityIdQueryHandler.cs` | `Application/Users/Queries/GetUserByIdentityIdQuery.cs` |
| Query | `GetUsersQuery` | `Application.Users.Queries` | `PaginatedApplicationResponse<ApplicationUser>` | `Application/Users/Handlers/GetUsersQueryHandler.cs` | `Application/Users/Queries/GetUsersQuery.cs` |



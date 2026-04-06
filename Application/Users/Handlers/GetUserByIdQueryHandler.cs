using Application.Contracts.Infrastructure;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Handlers
{
    public class GetUserByIdQueryHandler(IDatabase database)
        : IQueryHandler<GetUserByIdQuery, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(GetUserByIdQuery command, CancellationToken cancellationToken)
        {
            var user = await database.Users.FindAsync(command.Id);

            if (user is null)
                return Result.NotFound();

            return Result.Success(ApplicationUser.FromEntity(user));
        }
    }
}
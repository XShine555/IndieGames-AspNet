using Application.Contracts.Infrastructure;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Handlers
{
    public class GetUserByIdentityIdQueryHandler(IDatabase database)
        : IQueryHandler<GetUserByIdentityIdQuery, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(GetUserByIdentityIdQuery query, CancellationToken cancellationToken)
        {
            var user = await database.Users.FindAsync(query.IdentityId);

            if (user is null)
                return Result.NotFound();
            return Result.Success(ApplicationUser.FromEntity(user));
        }
    }
}
using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers
{
    public class GetBasicUserQueryHandler(IDatabase database, IUserMapper userMapper)
        : IQueryHandler<GetBasicUserQuery, Result<ApplicationBasicUser>>
    {
        public async ValueTask<Result<ApplicationBasicUser>> Handle(GetBasicUserQuery query, CancellationToken cancellationToken)
        {
            var user = await database.Users.AsNoTracking()
                .SingleOrDefaultAsync(u => u.IdentityId == query.UserId, cancellationToken);
            if (user is null)
                return Result.NotFound();

            return Result.Success(userMapper.ToApplicationBasicUser(user));
        }
    }
}
using System.Linq;
using app.Data;
using app.Models;
using HotChocolate;
using HotChocolate.Types;

namespace app.Types
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor
                .Field(user => user.PasswordHash)
                .Ignore();

            descriptor
                .Field(user => user.PhoneNumber)
                .Authorize(new[] { "moderator", "admin" }); // roles

            descriptor
                .Field(user => user.AccessFailedCount)
                .Authorize("RequireAdminRoleOrModerator"); // policy

            descriptor
                .Field(user => user.EmailConfirmed)
                .Authorize(); // basic auth

            descriptor
                .Field(p => p.Platforms)
                .ResolveWith<Resolvers>(p => p.GetPlatforms(default!, default!))
                .UseDbContext<DataContext>();
        }

        private class Resolvers
        {
            public IQueryable<Platform> GetPlatforms(User user, [ScopedService] DataContext context)
            {
                return context.Platforms.Where(platform => platform.UserId == user.Id);
            }
        }
    }
}
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

            // descriptor
                // .Field(user => user.PhoneNumber)
                // .Authorize() // basic auth
                // .Authorize(new [] {"foo", "bar"}); // roles
                // .Authorize("SalesDepartment"); // policy

            descriptor
                .Field(p => p.Platforms)
                .ResolveWith<Resolvers>(p => p.GetPlatforms(default!, default!))
                .UseDbContext<DataContext>();
        }

        private class Resolvers
        {
            public IQueryable<Platform> GetPlatforms(User user, [ScopedService] DataContext context)
            {
                return context.Platforms.Where(platform => platform.PlatformId == user.Id);
            }
        }
    }
}
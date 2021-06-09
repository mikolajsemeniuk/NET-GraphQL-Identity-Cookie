using System.Linq;
using app.Data;
using app.Models;
using HotChocolate;
using HotChocolate.Types;

namespace app.Types
{
    public class PlatformType : ObjectType<Platform>
    {
        protected override void Configure(IObjectTypeDescriptor<Platform> descriptor)
        {
            descriptor
                .Field(p => p.User)
                .ResolveWith<Resolvers>(p => p.GetUser(default!, default!))
                .UseDbContext<DataContext>();
        }

        private class Resolvers
        {
            public User GetUser(Platform platform, [ScopedService] DataContext context)
            {
                return context.Users.FirstOrDefault(user => user.Id == platform.UserId);
            }
        }
    }
}
using System.Linq;
using app.Data;
using app.Models;
using HotChocolate;
using HotChocolate.Data;

namespace app.GraphQL
{
    public class Query
    {
        [UseDbContext(typeof(DataContext))]
        public IQueryable<User> User([ScopedService] DataContext context) =>
            context.Users;

        [UseDbContext(typeof(DataContext))]
        public IQueryable<Platform> Platform([ScopedService] DataContext context) =>
            context.Platforms;
    }
}
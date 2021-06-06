using app.GraphQL.Payloads;
using app.Data;
using HotChocolate.Data;
using System.Threading.Tasks;
using HotChocolate;
using app.Models;

namespace app.GraphQL
{
    public class Mutation
    {
        
        [UseDbContext(typeof(DataContext))]
        public async Task SignUpUserAsync()
        {
            //
        }

        [UseDbContext(typeof(DataContext))]
        public async Task<AddPlatformPayload> AddPlatformAsync(AddPlatformInput input,
            [ScopedService] DataContext context)
        {
            var platform = new Platform
            {
                Name = input.Name
            };

            context.Platforms.Add(platform);

            await context.SaveChangesAsync();

            return new AddPlatformPayload(platform);
        }
    }
}
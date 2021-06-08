using app.Payloads;
using app.Data;
using HotChocolate.Data;
using System.Threading.Tasks;
using HotChocolate;
using app.Models;
using app.Inputs;
using Microsoft.AspNetCore.Http;
using HotChocolate.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using HotChocolate.Execution;

namespace app.GraphQL
{
    public class Mutation
    {

        [Authorize]
        [UseDbContext(typeof(DataContext))]
        public async Task<AddPlatformPayload> AddPlatformAsync(AddPlatformInput input,
            [Service] IHttpContextAccessor httpContext,
            [ScopedService] DataContext context)
        {
            var id = httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var platform = new Platform
            {
                Name = input.Name,
                UserId = Convert.ToInt32(id)
            };

            context.Platforms.Add(platform);

            if((await context.SaveChangesAsync()) < 1)
                throw new QueryException("something went wrong while saving data");

            return new AddPlatformPayload(platform);
        }
    }
}
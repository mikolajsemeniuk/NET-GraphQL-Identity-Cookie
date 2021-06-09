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
using Microsoft.EntityFrameworkCore;

namespace app.GraphQL
{
    public class Mutation
    {

        [Authorize]
        [UseDbContext(typeof(DataContext))]
        public async Task<PlatformPayload> AddPlatformAsync(AddPlatformInput input,
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

            if ((await context.SaveChangesAsync()) < 1)
                throw new QueryException("something went wrong while saving data");

            return new PlatformPayload(platform);
        }

        [Authorize]
        [UseDbContext(typeof(DataContext))]
        public async Task<PlatformPayload> UpdatePlatformAsync(UpdatePlatformInput input,
            [Service] IHttpContextAccessor httpContext,
            [ScopedService] DataContext context)
        {
            var id = httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var platform = await context.Platforms.FirstAsync(platform =>
                platform.PlatformId == input.id &&
                platform.UserId == Convert.ToInt32(id));

            if (platform == null)
                throw new QueryException("platfrom with this id not found");

            // jezeli rekord sie nie zmieni to ta funkcja SaveChangesAsync zwróci 0
            if (platform.Name == input.Name)
                return new PlatformPayload(platform);

            platform.Name = input.Name;

            if ((await context.SaveChangesAsync()) < 1)
                throw new QueryException("something went wrong while saving data");

            return new PlatformPayload(platform);
        }

        [Authorize]
        [UseDbContext(typeof(DataContext))]
        public async Task<PlatformPayload> RemovePlatformAsync(RemovePlatformInput input,
            [Service] IHttpContextAccessor httpContext,
            [ScopedService] DataContext context)
        {
            var id = httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var platform = await context.Platforms.FirstAsync(platform =>
                platform.PlatformId == input.id &&
                platform.UserId == Convert.ToInt32(id));

            if (platform == null)
                throw new QueryException("platfrom with this id not found");

            context.Platforms.Remove(platform);

            if ((await context.SaveChangesAsync()) < 1)
                throw new QueryException("something went wrong while saving data");

            return new PlatformPayload(platform);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class UsermanagerExtensions
    {
        public static async Task<AppUser> FindUserByClaimsPrincipalWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            return await userManager.Users.Include(u => u.Address).SingleOrDefaultAsync(u => u.Email == email);
        }

        public static async Task<AppUser> FindByEmailFromClaimPrincipal(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            return await userManager.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
    }
}
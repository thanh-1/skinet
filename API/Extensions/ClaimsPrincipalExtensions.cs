using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetreiveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            // return user?.Claims?.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;

            return user.FindFirstValue(ClaimTypes.Email);
        }
    }
}
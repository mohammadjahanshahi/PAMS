using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace BSIActivityManagement.Extensions
{
    public static class IdentityExtension
    {
        public static string GetImageId(this IPrincipal user)
        {// Thumbprint contains AMUserId
            try
            {
                var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Thumbprint);
                return claim == null ? null : Logic.DMLExt.GetImageIdByAmUserId(claim.Value);
            }
            catch
            {
                return null;
            }
        }

        public static string GetAmUser(this IPrincipal user)
        {
            try
            {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Thumbprint);
            return claim == null ? null : claim.Value;
            }
            catch
            {
                return null;
            }
        }

    }

}
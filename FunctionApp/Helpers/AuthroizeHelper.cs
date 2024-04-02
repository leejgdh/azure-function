using System.Security.Claims;

namespace FunctionApp.Helpers
{
    public static class AuthroizeHelper
    {

        public static bool TryGetClaim(this IEnumerable<Claim> claims, string type, out string value)
        {

            var claim = claims.FirstOrDefault(e => e.Type == type);


            if (claim == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = claim.Value;
                return true;
            }


        }
    }
}

using System.Security.Claims;

namespace psm_web_site_api_project.Utils.GetIdentities;

    public static class GetIdentitiesUser {
        public static string GetCurrentUserId(IEnumerable<ClaimsIdentity> values) {
            var identityUserId = values.FirstOrDefault().Claims.FirstOrDefault().Value;
            if (identityUserId == null) {
                return "";
            }
            return identityUserId;
        }
    }
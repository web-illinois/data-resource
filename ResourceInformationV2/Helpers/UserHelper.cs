using Microsoft.AspNetCore.Components.Authorization;

namespace ResourceInformationV2.Helpers {

    public static class UserHelper {

        public static async Task<string> GetUser(this AuthenticationStateProvider? provider) => provider == null ? "" : (await provider.GetAuthenticationStateAsync())?.User?.Identity?.Name ?? "";
    }
}
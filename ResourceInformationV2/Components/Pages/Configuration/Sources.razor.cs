using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Sources {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string NewSource { get; set; } = "";

        public string NewSourceCode { get; set; } = "";

        public IEnumerable<Tuple<string, string>> SourceEntries { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        public SourceHelper SourceHelper { get; set; } = default!;

        protected async Task CreateSource() {
            await Layout.AddMessage(await SourceHelper.CreateSource(NewSourceCode, NewSource, await UserHelper.GetUser(AuthenticationStateProvider)));
            var email = await UserHelper.GetUser(AuthenticationStateProvider);
            var baseUrl = await SourceHelper.GetBaseUrlFromSource(NewSourceCode);
            CacheHolder.SetCacheSource(email, NewSourceCode, baseUrl);
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            var sidebar = string.IsNullOrWhiteSpace(await Layout.CheckSource(false)) ? SidebarEnum.ConfigurationNoSource : SidebarEnum.Configuration;
            Layout.SetSidebar(sidebar, "Configuration");
            SourceEntries = await SourceHelper.GetSourcesAndOwners();
        }

        protected async Task RequestAccess(string key) => await Layout.AddMessage(await SourceHelper.RequestAccess(key, await UserHelper.GetUser(AuthenticationStateProvider)));
    }
}
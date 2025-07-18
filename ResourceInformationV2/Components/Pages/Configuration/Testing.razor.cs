using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Testing {
        private string _email = "";

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        public void SwitchToTesting() {
            CacheHolder.SetCacheSource(_email, "test", "");
            NavigationManager.NavigateTo("/configuration/security");
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            var sidebar = string.IsNullOrWhiteSpace(await Layout.CheckSource(false)) ? SidebarEnum.ConfigurationNoSource : SidebarEnum.Configuration;
            Layout.SetSidebar(sidebar, "Configuration");
            _email = await UserHelper.GetUser(AuthenticationStateProvider);
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
        }
    }
}
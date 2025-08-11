using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Templates {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
            var sourceCode = await Layout.CheckSource();
        }
    }
}
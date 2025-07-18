using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Layout {

    public partial class Breadcrumb {
        private List<PageLink>? _breadcrumbs = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        public void Rebuild(SidebarEnum s) {
            if (s != SidebarEnum.None) {
                _breadcrumbs = PageGroup.GetBreadcrumbs(s);
                StateHasChanged();
            }
        }
    }
}
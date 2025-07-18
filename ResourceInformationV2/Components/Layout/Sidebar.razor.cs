using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Layout {

    public partial class Sidebar {

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private string _baseUrl { get; set; } = "";
        private SidebarEnum _sidebar { get; set; } = default!;
        private List<PageLink>? _sidebarLinks { get; set; } = default!;
        private string _title { get; set; } = "";

        public void Rebuild(SidebarEnum s, string title) {
            _baseUrl = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            _title = title;
            _sidebar = s;
            _sidebarLinks = PageGroup.GetSidebar(s);
            StateHasChanged();
        }
    }
}
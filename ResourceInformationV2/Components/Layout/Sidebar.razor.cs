using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Layout {

    public partial class Sidebar {

        public void Rebuild(SidebarEnum s, string title) {
            StateHasChanged();
        }
    }
}
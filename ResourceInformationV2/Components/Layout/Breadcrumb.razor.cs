using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Layout {

    public partial class Breadcrumb {

        public void Rebuild(SidebarEnum s) {
            StateHasChanged();
        }
    }
}
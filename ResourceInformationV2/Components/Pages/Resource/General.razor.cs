using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class General {
        public Search.Models.Resource Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SourceCode { get; set; } = "";

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected ResourceSetter ResourceSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await ResourceSetter.SetItem(Item);
            await Layout.SetCacheId(Item.Id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, Item.Title);
            await Layout.Log(CategoryType.Resource, FieldType.General, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            SourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            var department = await Layout.ConfirmDepartmentName(false);
            if (!string.IsNullOrWhiteSpace(id)) {
                Item = await ResourceGetter.GetItem(id);
                Layout.SetSidebar(SidebarEnum.ResourceItem, Item.Title);
            } else {
                Item = new Search.Models.Resource {
                    Source = SourceCode,
                    IsActive = false,
                    DepartmentList = string.IsNullOrWhiteSpace(department) ? [] : [department]
                };
                Layout.SetSidebar(SidebarEnum.ResourceItem, "New " + Item.NameType, true);
            }
            await base.OnInitializedAsync();
        }
    }
}
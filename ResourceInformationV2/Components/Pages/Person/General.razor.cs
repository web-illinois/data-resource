using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Person {

    public partial class General {
        public Search.Models.Person Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SourceCode { get; set; } = "";

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected PersonGetter PersonGetter { get; set; } = default!;

        [Inject]
        protected PersonSetter PersonSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await PersonSetter.SetItem(Item);
            await Layout.SetCacheId(Item.Id);
            Layout.SetSidebar(SidebarEnum.PeopleItem, Item.Title);
            await Layout.Log(CategoryType.Person, FieldType.General, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            SourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();

            if (!string.IsNullOrWhiteSpace(id)) {
                Item = await PersonGetter.GetItem(id);
                Layout.SetSidebar(SidebarEnum.PeopleItem, Item.Title);
            } else {
                Item = new Search.Models.Person() {
                    Source = SourceCode,
                    IsActive = false
                };
                Layout.SetSidebar(SidebarEnum.PeopleItem, "New " + Item.NameType, true);
            }
            await base.OnInitializedAsync();
        }
    }
}
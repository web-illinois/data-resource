using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Person {

    public partial class Technical {
        public Search.Models.Person Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected PersonGetter PersonGetter { get; set; } = default!;

        [Inject]
        protected PersonSetter PersonSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await PersonSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Person, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/person/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await PersonSetter.SetItem(Item);
            await Layout.Log(CategoryType.Person, FieldType.Technical, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await PersonGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.PeopleItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}
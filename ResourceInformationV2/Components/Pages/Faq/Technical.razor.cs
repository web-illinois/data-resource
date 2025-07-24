using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Faq {

    public partial class Technical {
        public Search.Models.FaqItem Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected FaqGetter FaqGetter { get; set; } = default!;

        [Inject]
        protected FaqSetter FaqSetter { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await FaqSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Faq, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/resources/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await FaqSetter.SetItem(Item);
            await Layout.Log(CategoryType.Faq, FieldType.Technical, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await FaqGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.FaqItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}
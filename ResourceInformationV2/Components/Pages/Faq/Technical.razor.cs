using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Faq {

    public partial class Technical {
        private bool _originalStatus;
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
            NavigationManager.NavigateTo("/faq/edit");
        }

        public async Task Save() {
            if (Item.IsNewerDraft) {
                _ = await FaqSetter.PublishDraftItem(Item);
                await Layout.Log(CategoryType.Faq, FieldType.Technical, Item, "Publish Draft Item", EmailType.OnPublication);
                NavigationManager.NavigateTo("/faq/edit");
            } else {
                if (_originalStatus && !Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Faq, FieldType.Technical, Item, "Moved To Draft", EmailType.OnDraft);
                } else if (!_originalStatus && Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Faq, FieldType.Technical, Item, "Published", EmailType.OnPublication);
                } else {
                    await Layout.Log(CategoryType.Faq, FieldType.Technical, Item);
                }
                _ = await FaqSetter.SetItem(Item);
                _originalStatus = Item.IsActive;
                await Layout.AddMessage(Item.NameType + " saved successfully.");
            }
            Layout.RemoveDirty();
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            _originalStatus = Item.IsActive;
            Item = await FaqGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.FaqItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}
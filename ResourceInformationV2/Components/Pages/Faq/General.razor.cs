using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Faq {

    public partial class General {
        public Search.Models.FaqItem Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SourceCode { get; set; } = "";

        [Inject]
        protected FaqGetter FaqGetter { get; set; } = default!;

        [Inject]
        protected FaqSetter FaqSetter { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await FaqSetter.SetItem(Item);
            await Layout.SetCacheId(Item.Id);
            Layout.SetSidebar(SidebarEnum.FaqItem, Item.Title);
            await Layout.Log(CategoryType.Faq, FieldType.General, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            SourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();

            if (!string.IsNullOrWhiteSpace(id)) {
                Item = await FaqGetter.GetItem(id);
                Layout.SetSidebar(SidebarEnum.FaqItem, Item.Title);
            } else {
                Item = new Search.Models.FaqItem() {
                    Source = SourceCode,
                    IsActive = false
                };
                Layout.SetSidebar(SidebarEnum.FaqItem, "New " + Item.NameType, true);
            }
            await base.OnInitializedAsync();
        }
    }
}
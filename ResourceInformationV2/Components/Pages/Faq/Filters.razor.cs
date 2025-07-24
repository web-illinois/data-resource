using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Faq {

    public partial class Filters {
        private FilterListToggle _filterListToggle = default!;

        public Search.Models.FaqItem Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected FaqGetter FaqGetter { get; set; } = default!;

        [Inject]
        protected FaqSetter FaqSetter { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        public async Task Save() {
            Item.AudienceList = _filterListToggle.AudienceTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.TopicList = _filterListToggle.TopicTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.TagList = _filterListToggle.Tags1?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Layout.RemoveDirty();
            await Layout.AddMessage(Item.NameType + " saved successfully.");
            await Layout.Log(CategoryType.Faq, FieldType.Filters, Item);
            _ = await FaqSetter.SetItem(Item);
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            Item = await FaqGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.FaqItem, Item.Title);
        }
    }
}
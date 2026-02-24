using Blazored.TextEditor;
using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Faq {

    public partial class Detail {
        private BlazoredTextEditor _rteDescription;
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
            if (_rteDescription != null) {
                Item.DetailAnswer = await _rteDescription.GetHTML();
            }

            _ = await FaqSetter.SetItem(Item);
            await Layout.Log(CategoryType.Faq, FieldType.Specific, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                var id = await Layout.GetCachedId();
                Item = await FaqGetter.GetItem(id);
                await _rteDescription.LoadHTMLContent(Item.DetailAnswer);
                Layout.SetSidebar(SidebarEnum.FaqItem, Item.Title);
            }
        }
    }
}
using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Note {

    public partial class Detail {
        private RichTextEditor _rteDescription = default!;
        public Search.Models.NoteItem Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SourceCode { get; set; } = "";

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NoteGetter NoteGetter { get; set; } = default!;

        [Inject]
        protected NoteSetter NoteSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Save() {
            Layout.RemoveDirty();
            if (_rteDescription != null) {
                Item.DetailText = await _rteDescription.GetValue();
            }

            _ = await NoteSetter.SetItem(Item);
            await Layout.Log(CategoryType.Note, FieldType.Specific, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            Item = await NoteGetter.GetItem(id);
            await _rteDescription.Load(Item.DetailText);
            Layout.SetSidebar(SidebarEnum.NotesItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}
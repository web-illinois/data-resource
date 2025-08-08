using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Note {

    public partial class Filters {
        private FilterListToggle _filterListToggle = default!;

        public Search.Models.NoteItem Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NoteGetter NoteGetter { get; set; } = default!;

        [Inject]
        protected NoteSetter NoteSetter { get; set; } = default!;

        public async Task Save() {
            Item.AudienceList = _filterListToggle.AudienceTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.TopicList = _filterListToggle.TopicTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.TagList = _filterListToggle.Tags1?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Layout.RemoveDirty();
            await Layout.AddMessage(Item.NameType + " saved successfully.");
            await Layout.Log(CategoryType.Note, FieldType.Filters, Item);
            _ = await NoteSetter.SetItem(Item);
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            Item = await NoteGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.NotesItem, Item.Title);
        }
    }
}
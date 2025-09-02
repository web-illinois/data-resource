using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Review {

    public partial class ViewDraft {

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public List<GenericItem> Listing { get; set; } = default!;

        [Inject]
        protected EventGetter EventGetter { get; set; } = default!;

        [Inject]
        protected FaqGetter FaqGetter { get; set; } = default!;

        [Inject]
        protected NoteGetter NoteGetter { get; set; } = default!;

        [Inject]
        protected PersonGetter PersonGetter { get; set; } = default!;

        [Inject]
        protected PublicationGetter PublicationGetter { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.Review, "Review Items");
            var sourceCode = await Layout.CheckSource();
            Listing = [.. await ResourceGetter.GetAllDraftItemsBySource(sourceCode), .. await PublicationGetter.GetAllDraftItemsBySource(sourceCode),
                .. await NoteGetter.GetAllDraftItemsBySource(sourceCode), .. await FaqGetter.GetAllDraftItemsBySource(sourceCode),
                .. await EventGetter.GetAllDraftItemsBySource(sourceCode), .. await PersonGetter.GetAllDraftItemsBySource(sourceCode)];
        }
    }
}
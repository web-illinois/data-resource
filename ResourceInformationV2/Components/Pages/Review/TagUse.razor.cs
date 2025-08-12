using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.JsonThinModels;

namespace ResourceInformationV2.Components.Pages.Review {

    public partial class TagUse {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public List<TagList> Tags { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected EventGetter EventGetter { get; set; } = default!;

        [Inject]
        protected FaqGetter FaqGetter { get; set; } = default!;

        [Inject]
        protected FilterTranslator FilterTranslator { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NoteGetter NoteGetter { get; set; } = default!;

        [Inject]
        protected PersonGetter PersonGetter { get; set; } = default!;

        [Inject]
        protected PublicationGetter PublicationGetter { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            Layout.SetSidebar(SidebarEnum.Review, "Review Items");
            var sourceCode = await Layout.CheckSource();
            var tagTitles = await FilterTranslator.GetTagTitles(sourceCode);
            Tags = [.. Convert(await EventGetter.GetTagCount(sourceCode), "Events", tagTitles),
                .. Convert(await FaqGetter.GetTagCount(sourceCode), "FAQs", tagTitles),
                .. Convert(await NoteGetter.GetTagCount(sourceCode), "Notes", tagTitles),
                .. Convert(await PersonGetter.GetTagCount(sourceCode), "People", tagTitles),
                .. Convert(await PublicationGetter.GetTagCount(sourceCode), "Publications", tagTitles),
                .. Convert(await ResourceGetter.GetTagCount(sourceCode), "Resources", tagTitles)];
        }

        private List<TagList> Convert(List<TagList> lists, string mainTitle, Dictionary<string, string> tagTitles) {
            foreach (var list in lists) {
                list.Title = mainTitle + " " + tagTitles[list.Title];
            }
            return lists;
        }
    }
}
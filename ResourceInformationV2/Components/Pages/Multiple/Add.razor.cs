using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Pages.Multiple {

    public partial class Add {
        public bool UseEvents;
        public bool UseFaqs;
        public bool UseNotes;
        public bool UsePeople;
        public bool UseOrgChart;
        public bool UsePublications;
        public bool UseResources;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.AddEditInformation, "Add / Edit Items");
            var sourceCode = await Layout.CheckSource();
            await Layout.ClearCacheId();
            (UseEvents, UseFaqs, UseNotes, UseResources, UsePeople, UsePublications, UseOrgChart) = await SourceHelper.DoesSourceUseItemCheckAll(sourceCode);
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/configuration/sources");
            }
            if (UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/event/general");
            }
            if (!UseEvents && UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/faq/general");
            }
            if (!UseEvents && !UseFaqs && UseNotes && !UseResources && !UsePeople && !UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/notes/general");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && UseResources && !UsePeople && !UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/resource/general");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && UsePeople && !UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/person/general");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && UsePublications && !UseOrgChart) {
                NavigationManager.NavigateTo("/publication/general");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications && UseOrgChart) {
                NavigationManager.NavigateTo("/orgchart/general");
            }
            await base.OnInitializedAsync();
        }
    }
}
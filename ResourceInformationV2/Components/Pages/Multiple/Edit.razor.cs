using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Pages.Multiple {

    public partial class Edit {
        public bool UseEvents;
        public bool UseFaqs;
        public bool UseNotes;
        public bool UsePeople;
        public bool UsePublications;
        public bool UseResources;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.AddEditInformation, "Resources");
            var sourceCode = await Layout.CheckSource();
            (UseEvents, UseFaqs, UseNotes, UseResources, UsePeople, UsePublications) = await SourceHelper.DoesSourceUseItemCheckAll(sourceCode);
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications) {
                NavigationManager.NavigateTo("/configuration/sources");
            }
            if (UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications) {
                NavigationManager.NavigateTo("/event/edit");
            }
            if (!UseEvents && UseFaqs && !UseNotes && !UseResources && !UsePeople && !UsePublications) {
                NavigationManager.NavigateTo("/faq/edit");
            }
            if (!UseEvents && !UseFaqs && UseNotes && !UseResources && !UsePeople && !UsePublications) {
                NavigationManager.NavigateTo("/notes/edit");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && UseResources && !UsePeople && !UsePublications) {
                NavigationManager.NavigateTo("/resource/edit");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && UsePeople && !UsePublications) {
                NavigationManager.NavigateTo("/person/edit");
            }
            if (!UseEvents && !UseFaqs && !UseNotes && !UseResources && !UsePeople && UsePublications) {
                NavigationManager.NavigateTo("/publication/edit");
            }
            await base.OnInitializedAsync();
        }
    }
}
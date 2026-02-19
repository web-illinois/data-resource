using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using Model = ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Components.Pages.Review {

    public partial class LinkCheck {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected LinkCheckHelper LinkCheckHelper { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        public int LinksLeftToCheck { get; set; }

        public DateTime? DateLastLinkCheck { get; set; }

        public bool IsInProcess => LinksLeftToCheck > 0;

        public string DateLastLinkCheckFormatted => DateLastLinkCheck.HasValue ? DateLastLinkCheck.Value.ToString("g") : "Never";

        public List<Model.LinkCheck> Links { get; set; } = [];

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            Layout.SetSidebar(SidebarEnum.Review, "Review Items");
            await ListSchedule();
        }

        protected async Task ScheduleLinkCheck() {
            var sourceCode = await Layout.CheckSource();
            _ = await LinkCheckHelper.AddResources(sourceCode);
            await ListSchedule();
            StateHasChanged();
        }

        protected async Task ListSchedule() {
            var sourceCode = await Layout.CheckSource();
            (LinksLeftToCheck, DateLastLinkCheck, Links) = await LinkCheckHelper.GetLinkCheckStatus(sourceCode);
            StateHasChanged();
        }

        protected async Task CheckLinks() {
            _ = await LinkCheckHelper.CheckLink(10);
            await ListSchedule();
        }
    }
}
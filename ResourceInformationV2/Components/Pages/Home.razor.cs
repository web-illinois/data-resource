using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Helpers;

namespace ResourceInformationV2.Components.Pages {

    public partial class Home {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        public int NumberOfSources { get; set; }

        [SupplyParameterFromQuery]
        public bool RedirectIfNoSource { get; set; } = false;

        public string SelectedSource { get; set; } = "";
        public string SelectedSourceTitle { get; set; } = "";
        public bool UseEvents { get; set; }
        public bool UseFaqs { get; set; }
        public bool UseNotes { get; set; }
        public bool UsePeople { get; set; }
        public bool UsePublications { get; set; }
        public bool UseResources { get; set; }

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected Dictionary<string, string> Sources { get; set; } = default!;

        protected async Task ChangeSource(ChangeEventArgs e) {
            SelectedSource = e.Value?.ToString() ?? "";
            SelectedSourceTitle = Sources[SelectedSource];
            var email = await UserHelper.GetUser(AuthenticationStateProvider);
            var baseUrl = await SourceHelper.GetBaseUrlFromSource(SelectedSource);
            CacheHolder.SetCacheSource(email, SelectedSource, baseUrl);
            RedirectIfNoSource = false;
            await ChangeBoxes();
        }

        protected override async Task OnInitializedAsync() {
            var email = await UserHelper.GetUser(AuthenticationStateProvider);
            Sources = await SourceHelper.GetSources(email);
            NumberOfSources = Sources.Count;
            if (NumberOfSources == 1) {
                SelectedSource = Sources.First().Key;
                SelectedSourceTitle = Sources[SelectedSource];
                var baseUrl = await SourceHelper.GetBaseUrlFromSource(SelectedSource);
                CacheHolder.SetCacheSource(email, SelectedSource, baseUrl);
            } else if (CacheHolder.HasCachedItem(email)) {
                SelectedSource = CacheHolder.GetCacheSource(email) ?? "";
                SelectedSourceTitle = Sources[SelectedSource];
            }
            await ChangeBoxes();
            await base.OnInitializedAsync();
        }

        private async Task ChangeBoxes() {
            if (!string.IsNullOrWhiteSpace(SelectedSource)) {
                UseNotes = await SourceHelper.DoesSourceUseItem(SelectedSource, CategoryType.Note);
                UsePeople = await SourceHelper.DoesSourceUseItem(SelectedSource, CategoryType.Person);
                UseFaqs = await SourceHelper.DoesSourceUseItem(SelectedSource, CategoryType.Faq);
                UsePublications = await SourceHelper.DoesSourceUseItem(SelectedSource, CategoryType.Publication);
                UseResources = await SourceHelper.DoesSourceUseItem(SelectedSource, CategoryType.Resource);
                UseEvents = await SourceHelper.DoesSourceUseItem(SelectedSource, CategoryType.Event);
            }
        }
    }
}
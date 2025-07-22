using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Publication {

    public partial class Edit {
        private SearchGenericItem _searchGenericItem = default!;

        private string _sourceCode = "";
        private bool? _usePublications;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public List<GenericItem> PublicationList { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected PublicationGetter PublicationGetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected async Task ChoosePublication() {
            if (!string.IsNullOrWhiteSpace(_searchGenericItem.SelectedItemId)) {
                await Layout.SetCacheId(_searchGenericItem.SelectedItemId);
                NavigationManager.NavigateTo("/publication/general");
            }
        }

        protected async Task GetPublications() {
            PublicationList = await PublicationGetter.GetAllItemsBySource(_sourceCode, _searchGenericItem == null ? "" : _searchGenericItem.SearchItem);
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.AddEditInformation, "Publications");
            _sourceCode = await Layout.CheckSource();
            _usePublications = await SourceHelper.DoesSourceUseItem(_sourceCode, CategoryType.Publication);
            await GetPublications();
            await base.OnInitializedAsync();
        }

        protected async Task SetNewPublication() {
            await Layout.ClearCacheId();
            NavigationManager.NavigateTo("/publication/general");
        }
    }
}
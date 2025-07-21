using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class Edit {
        private SearchGenericItem _searchGenericItem = default!;

        private string _sourceCode = "";
        private bool? _useResources;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public List<GenericItem> ResourceList { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected async Task ChooseResource() {
            if (!string.IsNullOrWhiteSpace(_searchGenericItem.SelectedItemId)) {
                await Layout.SetCacheId(_searchGenericItem.SelectedItemId);
                NavigationManager.NavigateTo("/resource/general");
            }
        }

        protected async Task GetResources() {
            ResourceList = await ResourceGetter.GetAllItemsBySource(_sourceCode, _searchGenericItem == null ? "" : _searchGenericItem.SearchItem);
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.AddEditInformation, "Resources");
            _sourceCode = await Layout.CheckSource();
            _useResources = await SourceHelper.DoesSourceUseItem(_sourceCode, CategoryType.Resource);
            await GetResources();
            await base.OnInitializedAsync();
        }

        protected async Task SetNewResource() {
            await Layout.ClearCacheId();
            NavigationManager.NavigateTo("/resource/general");
        }
    }
}
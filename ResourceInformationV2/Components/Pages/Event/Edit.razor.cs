using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Event {

    public partial class Edit {
        private SearchGenericItem _searchGenericItem = default!;

        private string _sourceCode = "";
        private bool? _useItem;

        public List<GenericItem> ItemList { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected EventGetter EventGetter { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected async Task ChooseItem() {
            if (!string.IsNullOrWhiteSpace(_searchGenericItem.SelectedItemId)) {
                await Layout.SetCacheId(_searchGenericItem.SelectedItemId);
                NavigationManager.NavigateTo("/event/general");
            }
        }

        protected async Task GetItems() {
            var department = await Layout.ConfirmDepartmentName(false);
            ItemList = await EventGetter.GetAllItemsBySource(_sourceCode, _searchGenericItem == null ? "" : _searchGenericItem.SearchItem, department);
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.AddEditInformation, "Events");
            _sourceCode = await Layout.CheckSource();
            _useItem = await SourceHelper.DoesSourceUseItem(_sourceCode, CategoryType.Faq);
            await GetItems();
            await base.OnInitializedAsync();
        }

        protected async Task SetNewItem() {
            await Layout.ClearCacheId();
            NavigationManager.NavigateTo("/event/general");
        }
    }
}
using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Controls {

    public partial class SearchGenericItem {

        [Parameter]
        public EventCallback<string> AddClicked { get; set; }

        [Parameter]
        public EventCallback<string> EditClicked { get; set; }

        [Parameter]
        public List<GenericItem> GenericItems { get; set; } = default!;

        public bool IsEditDisabled => SelectedItemId == null || string.IsNullOrWhiteSpace(SelectedItemId);

        [Parameter]
        public string ItemTitle { get; set; } = "";

        [Parameter]
        public EventCallback<string> SearchClicked { get; set; }

        [Parameter]
        public string SearchItem { get; set; } = "";

        public string SelectedItemId { get; set; } = "";

        public string SelectedItemTitle => GenericItems.FirstOrDefault(gi => gi.Id == SelectedItemId)?.Title ?? "";

        public void Add() {
            AddClicked.InvokeAsync();
        }

        public void Edit() {
            if (GenericItems.Count == 1) {
                SelectedItemId = GenericItems[0].Id;
            }
            EditClicked.InvokeAsync();
        }

        public void Search() {
            SelectedItemId = "";
            SearchClicked.InvokeAsync();
        }

        protected async Task FilterChange(ChangeEventArgs e) {
            SearchItem = e.Value?.ToString() ?? "";
            SelectedItemId = "";
            await SearchClicked.InvokeAsync();
            if (GenericItems.Count == 1) {
                SelectedItemId = GenericItems[0].Id;
            }
        }
    }
}
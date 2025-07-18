using Microsoft.AspNetCore.Components;

namespace ResourceInformationV2.Components.Controls {

    public partial class DeleteNotice {
        public bool DeleteCheckbox { get; set; } = false;

        [Parameter]
        public EventCallback<string> DeleteClicked { get; set; }

        [Parameter]
        public string ItemId { get; set; } = "Id";

        [Parameter]
        public string ItemText { get; set; } = "";

        [Parameter]
        public string ItemTitle { get; set; } = "";

        public void Delete() {
            DeleteClicked.InvokeAsync();
        }
    }
}
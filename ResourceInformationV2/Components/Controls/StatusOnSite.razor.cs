using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;

namespace ResourceInformationV2.Components.Controls {

    public partial class StatusOnSite {
        private bool _value;

        [Parameter]
        public bool IsNewerDraft { get; set; } = false;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Parameter]
        public bool Value {
            get => _value;
            set {
                if (_value == value)
                    return;
                if (Layout != null)
                    Layout.SetDirty();
                _value = value;
                ValueChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<bool> ValueChanged { get; set; }
    }
}
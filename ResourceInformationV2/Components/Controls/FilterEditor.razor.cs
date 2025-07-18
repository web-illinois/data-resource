using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Components.Controls {

    public partial class FilterEditor {
        private Tag? _value;

        [Parameter]
        public string FilterType { get; set; } = "";

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public EventCallback<Tag> MoveDownCallback { get; set; }

        [Parameter]
        public EventCallback<Tag> MoveUpCallback { get; set; }

        [Parameter]
        public EventCallback<Tag> RemoveCallback { get; set; }

        [Parameter]
        public string Title { get; set; } = default!;

        [Parameter]
        public Tag? Value {
            get => _value;
            set {
                if (_value == value)
                    return;

                _value = value;
                ValueChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<Tag> ValueChanged { get; set; }

        public async Task MoveDown() => await MoveDownCallback.InvokeAsync(Value);

        public async Task MoveUp() => await MoveUpCallback.InvokeAsync(Value);

        public async Task Remove() => await RemoveCallback.InvokeAsync(Value);
    }
}
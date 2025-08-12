using Microsoft.AspNetCore.Components;

namespace ResourceInformationV2.Components.Controls {

    public partial class FilterGroupEditor {
        private Tuple<string, string>? _value;

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public EventCallback<Tuple<string, string>> MoveDownCallback { get; set; }

        [Parameter]
        public EventCallback<Tuple<string, string>> MoveUpCallback { get; set; }

        [Parameter]
        public Tuple<string, string>? Value {
            get => _value;
            set {
                if (_value == value)
                    return;

                _value = value;
                ValueChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<Tuple<string, string>> ValueChanged { get; set; }

        public async Task MoveDown() => await MoveDownCallback.InvokeAsync(Value);

        public async Task MoveUp() => await MoveUpCallback.InvokeAsync(Value);
    }
}
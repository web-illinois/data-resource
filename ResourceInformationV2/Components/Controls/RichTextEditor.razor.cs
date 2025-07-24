using Blazored.TextEditor;
using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;

namespace ResourceInformationV2.Components.Controls {

    public partial class RichTextEditor {
        private BlazoredTextEditor? _quillItem = default!;

        public string Id => Title.Replace(" ", "_").ToLowerInvariant();

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Parameter]
        public EventCallback<string> OnInitializedCallback { get; set; }

        [Parameter]
        public string Title { get; set; } = "";

        public async Task<string> GetValue() {
            if (_quillItem == null) {
                return "";
            }
            var returnValue = await _quillItem.GetHTML();
            if (!returnValue.Contains("<p")) {
                return $"<p>{returnValue}</p>";
            }
            return returnValue;
        }

        public async Task Load(string s) {
            if (_quillItem != null) {
                await _quillItem.LoadHTMLContent(s);
            }
        }
    }
}
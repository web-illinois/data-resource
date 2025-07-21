using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Components.Controls {

    public partial class Instruction {

        [Parameter]
        public CategoryType CategoryType { get; set; } = CategoryType.None;

        [Parameter]
        public FieldType FieldType { get; set; } = FieldType.None;

        [Inject]
        public InstructionHelper InstructionHelper { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; }

        public bool Show => !string.IsNullOrWhiteSpace(Text);

        public string Text { get; set; } = "";

        protected override async Task OnInitializedAsync() {
            Text = await InstructionHelper.GetInstructionText(Layout.SourceCode, CategoryType, FieldType);
            _ = base.OnInitializedAsync();
        }
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Controls {

    public partial class InstructionList {

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Parameter]
        public CategoryType CategoryType { get; set; } = CategoryType.None;

        [Inject]
        public InstructionHelper InstructionHelper { get; set; } = default!;

        public string InstructionsFilters { get; set; } = "";
        public string InstructionsGeneral { get; set; } = "";
        public string InstructionsImage { get; set; } = "";
        public string InstructionsLinks { get; set; } = "";
        public string InstructionsSpecific { get; set; } = "";
        public string InstructionsTechnical { get; set; } = "";
        public bool IsUsed { get; set; }
        public bool UseFragment { get; set; }

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        public SourceHelper SourceHelper { get; set; } = default!;

        public string UrlTemplate { get; set; } = "";

        public async Task SaveChanges() {
            Layout.RemoveDirty();
            var sourceCode = await Layout.CheckSource();
            var id = await SourceHelper.SetSourceItem(sourceCode, CategoryType, IsUsed, UseFragment);
            _ = await InstructionHelper.SaveInstructions(InstructionsGeneral, id, CategoryType, FieldType.General);
            _ = await InstructionHelper.SaveInstructions(InstructionsFilters, id, CategoryType, FieldType.Filters);
            _ = await InstructionHelper.SaveInstructions(InstructionsImage, id, CategoryType, FieldType.ImageAndVideo);
            _ = await InstructionHelper.SaveInstructions(InstructionsLinks, id, CategoryType, FieldType.Links);
            _ = await InstructionHelper.SaveInstructions(InstructionsTechnical, id, CategoryType, FieldType.Technical);
            if (CategoryType != CategoryType.Resource) {
                _ = await InstructionHelper.SaveInstructions(InstructionsSpecific, id, CategoryType, FieldType.Specific);
            }

            await Layout.AddMessage("Information saved");
        }

        public void SaveUsedChange(bool b) {
            IsUsed = b;
            Layout.SetDirty();
        }
        public void SaveUsedFragment(bool b) {
            UseFragment = b;
            Layout.SetDirty();
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            var sourceCode = await Layout.CheckSource();
            Layout.SetSidebar(SidebarEnum.Instructions, "Options and Instructions");
            var (sourceInstructions, isUsed, useFragment) = await InstructionHelper.GetInstructions(sourceCode, CategoryType);
            IsUsed = isUsed;
            UseFragment = useFragment;
            foreach (var instruction in sourceInstructions) {
                switch (instruction.FieldType) {
                    case FieldType.General:
                        InstructionsGeneral = instruction.Text ?? "";
                        break;

                    case FieldType.Filters:
                        InstructionsFilters = instruction.Text ?? "";
                        break;

                    case FieldType.ImageAndVideo:
                        InstructionsImage = instruction.Text ?? "";
                        break;

                    case FieldType.Links:
                        InstructionsLinks = instruction.Text ?? "";
                        break;

                    case FieldType.Technical:
                        InstructionsTechnical = instruction.Text ?? "";
                        break;
                }
            }
        }
    }
}
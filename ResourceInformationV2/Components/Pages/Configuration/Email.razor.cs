using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Email {
        public string EmailOption { get; set; } = "";

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public SourceEmail? SourceEmail { get; set; }

        [Inject]
        public SourceEmailHelper SourceEmailHelper { get; set; } = default!;

        public async Task ChangeEmailOption(ChangeEventArgs args) {
            var emailOption = Enum.Parse<EmailType>(args.Value?.ToString() ?? "None");
            SourceEmail = emailOption == EmailType.None ? null : await SourceEmailHelper.GetSourceEmail(await Layout.CheckSource(), emailOption);
        }

        public async Task Delete() {
            if (SourceEmail != null) {
                Layout.RemoveDirty();
                _ = await SourceEmailHelper.DeleteSourceEmail(SourceEmail);
                SourceEmail = null;
                await Layout.AddMessage("Your email has been removed");
            }
        }

        public async Task Save() {
            if (SourceEmail != null) {
                Layout.RemoveDirty();
                _ = await SourceEmailHelper.SaveSourceEmail(SourceEmail);
                await Layout.AddMessage("Your email has been configured successfully");
            }
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            await Layout.CheckSource();
            var sourceCode = await Layout.CheckSource();
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
        }
    }
}
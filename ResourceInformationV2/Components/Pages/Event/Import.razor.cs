using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.EventTranslator;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Event {
    public partial class Import {
        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;
        [Inject]
        protected EventSetter EventSetter { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        [Inject]
        protected IcsHolder IcsHolder { get; set; } = default!;

        public string Output { get; set; } = "";

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.AddEditInformation, "Events");
            await base.OnInitializedAsync();
        }

        public async Task ImportEvents() {
            IcsHolder.UploadUrl = "https://outlook.office365.com/owa/calendar/4f3ab5aa53ec43e48417f7336ed48bc4@illinois.edu/19c13fd55e264ee58dfb0556605c9c6b16245358679972666607/calendar.ics";
            IcsHolder.Source = await Layout.CheckSource();
            var results = await IcsHolder.Load();
            foreach (var eventItem in IcsHolder.Events) {
                _ = await EventSetter.SetItem(eventItem);
            }
            Output = $"Imported {IcsHolder.Events.Count} events.";
        }
    }
}

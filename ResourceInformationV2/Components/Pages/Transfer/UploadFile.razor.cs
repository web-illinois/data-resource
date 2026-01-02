using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Helpers;
using ResourceInformationV2.Search.Models;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Transfer {

    public partial class UploadFile {
        public List<string> Options = new();
        private const int _maxFileSize = 2048000;
        private string _reader = "";

        public bool DeleteAllFirst { get; set; }

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SelectedOption { get; set; } = "";
        public bool ValidateTags { get; set; }

        [Inject]
        protected BulkEditor BulkEditor { get; set; } = default!;

        [Inject]
        protected EventSetter EventSetter { get; set; } = default!;

        [Inject]
        protected FaqSetter FaqSetter { get; set; } = default!;

        [Inject]
        protected FilterHelper FilterHelper { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NoteSetter NoteSetter { get; set; } = default!;

        [Inject]
        protected PersonSetter PersonSetter { get; set; } = default!;

        [Inject]
        protected PublicationSetter PublicationSetter { get; set; } = default!;

        [Inject]
        protected ResourceSetter ResourceSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            ValidateTags = true;
            Layout.SetSidebar(SidebarEnum.Transfer, "Transfer Items");
            string sourceCode = await Layout.CheckSource();
            Options = [.. (await SourceHelper.DoesSourceUseItemCheckAllAndConcatenate(sourceCode))];
            SelectedOption = Options.First();
        }

        private async Task LoadFile(InputFileChangeEventArgs e) {
            _reader = await new StreamReader(e.File.OpenReadStream(_maxFileSize)).ReadToEndAsync();
            await Layout.AddMessage("File loaded for " + SelectedOption + " and ready to be uploaded");
        }

        private async Task Upload() {
            if (!string.IsNullOrWhiteSpace(_reader)) {
                await Layout.AddMessage("In process of being uploaded -- please do not leave this page");
                string? sourceCode = await Layout.CheckSource();
                (List<Tag> TagSources, int SourceId) t = await FilterHelper.GetFilters(sourceCode, TagType.Tag1);
                int sourceId = t.SourceId;
                List<Tag> tag1 = t.TagSources;
                List<Tag> tag2 = (await FilterHelper.GetFilters(sourceCode, TagType.Tag2)).TagSources;
                List<Tag> tag3 = (await FilterHelper.GetFilters(sourceCode, TagType.Tag3)).TagSources;
                List<Tag> tag4 = (await FilterHelper.GetFilters(sourceCode, TagType.Tag4)).TagSources;
                List<Tag> audience = (await FilterHelper.GetFilters(sourceCode, TagType.Audience)).TagSources;
                List<Tag> topic = (await FilterHelper.GetFilters(sourceCode, TagType.Topic)).TagSources;
                List<Tag> department = (await FilterHelper.GetFilters(sourceCode, TagType.Department)).TagSources;
                string displayString = DeleteAllFirst ? "Existing records deleted. " : "";
                List<string> newTag1 = new List<string>();
                List<string> newTag2 = new List<string>();
                List<string> newTag3 = new List<string>();
                List<string> newTag4 = new List<string>();
                List<string> newAudience = new List<string>();
                List<string> newTopic = new List<string>();
                List<string> newDepartment = new List<string>();

                _ = Enum.TryParse(SelectedOption, out UrlTypes urlType);

                switch (urlType) {
                    case UrlTypes.Resources:
                        if (DeleteAllFirst) {
                            _ = await BulkEditor.DeleteResources(sourceCode);
                        }
                        displayString += await ResourceSetter.UploadFile(sourceCode ?? "", _reader, ValidateTags, tag1.Select(t => t.Title), tag2.Select(t => t.Title), tag3.Select(t => t.Title), tag4.Select(t => t.Title), audience.Select(t => t.Title), topic.Select(t => t.Title), department.Select(t => t.Title));
                        newTag1 = ResourceSetter.AddedTag;
                        newTag2 = ResourceSetter.AddedTag2;
                        newTag3 = ResourceSetter.AddedTag3;
                        newTag4 = ResourceSetter.AddedTag4;
                        newAudience = ResourceSetter.AddedAudience;
                        newTopic = ResourceSetter.AddedTopic;
                        newDepartment = ResourceSetter.AddedDepartment;
                        break;

                    case UrlTypes.Events:
                        if (DeleteAllFirst) {
                            _ = await BulkEditor.DeleteEvents(sourceCode);
                        }
                        displayString += await EventSetter.UploadFile(sourceCode ?? "", _reader, ValidateTags, tag1.Select(t => t.Title), tag2.Select(t => t.Title), tag3.Select(t => t.Title), tag4.Select(t => t.Title), audience.Select(t => t.Title), topic.Select(t => t.Title), department.Select(t => t.Title));
                        newTag1 = EventSetter.AddedTag;
                        newTag2 = EventSetter.AddedTag2;
                        newTag3 = EventSetter.AddedTag3;
                        newTag4 = EventSetter.AddedTag4;
                        newAudience = EventSetter.AddedAudience;
                        newTopic = EventSetter.AddedTopic;
                        newDepartment = EventSetter.AddedDepartment;
                        break;

                    case UrlTypes.Faqs:
                        if (DeleteAllFirst) {
                            _ = await BulkEditor.DeleteFaqs(sourceCode);
                        }
                        displayString += await FaqSetter.UploadFile(sourceCode ?? "", _reader, ValidateTags, tag1.Select(t => t.Title), tag2.Select(t => t.Title), tag3.Select(t => t.Title), tag4.Select(t => t.Title), audience.Select(t => t.Title), topic.Select(t => t.Title), department.Select(t => t.Title));
                        newTag1 = FaqSetter.AddedTag;
                        newTag2 = FaqSetter.AddedTag2;
                        newTag3 = FaqSetter.AddedTag3;
                        newTag4 = FaqSetter.AddedTag4;
                        newAudience = FaqSetter.AddedAudience;
                        newTopic = FaqSetter.AddedTopic;
                        newDepartment = FaqSetter.AddedDepartment;
                        break;

                    case UrlTypes.Notes:
                        if (DeleteAllFirst) {
                            _ = await BulkEditor.DeleteNotes(sourceCode);
                        }
                        displayString += await NoteSetter.UploadFile(sourceCode ?? "", _reader, ValidateTags, tag1.Select(t => t.Title), tag2.Select(t => t.Title), tag3.Select(t => t.Title), tag4.Select(t => t.Title), audience.Select(t => t.Title), topic.Select(t => t.Title), department.Select(t => t.Title));
                        newTag1 = NoteSetter.AddedTag;
                        newTag2 = NoteSetter.AddedTag2;
                        newTag3 = NoteSetter.AddedTag3;
                        newTag4 = NoteSetter.AddedTag4;
                        newAudience = NoteSetter.AddedAudience;
                        newTopic = NoteSetter.AddedTopic;
                        newDepartment = NoteSetter.AddedDepartment;
                        break;

                    case UrlTypes.Publications:
                        if (DeleteAllFirst) {
                            _ = await BulkEditor.DeletePublications(sourceCode);
                        }
                        displayString += await PublicationSetter.UploadFile(sourceCode ?? "", _reader, ValidateTags, tag1.Select(t => t.Title), tag2.Select(t => t.Title), tag3.Select(t => t.Title), tag4.Select(t => t.Title), audience.Select(t => t.Title), topic.Select(t => t.Title), department.Select(t => t.Title));
                        newTag1 = PublicationSetter.AddedTag;
                        newTag2 = PublicationSetter.AddedTag2;
                        newTag3 = PublicationSetter.AddedTag3;
                        newTag4 = PublicationSetter.AddedTag4;
                        newAudience = PublicationSetter.AddedAudience;
                        newTopic = PublicationSetter.AddedTopic;
                        newDepartment = PublicationSetter.AddedDepartment;
                        break;

                    case UrlTypes.People:
                        if (DeleteAllFirst) {
                            _ = await BulkEditor.DeletePeople(sourceCode);
                        }
                        displayString += await PersonSetter.UploadFile(sourceCode ?? "", _reader, ValidateTags, tag1.Select(t => t.Title), tag2.Select(t => t.Title), tag3.Select(t => t.Title), tag4.Select(t => t.Title), audience.Select(t => t.Title), topic.Select(t => t.Title), department.Select(t => t.Title));
                        newTag1 = PersonSetter.AddedTag;
                        newTag2 = PersonSetter.AddedTag2;
                        newTag3 = PersonSetter.AddedTag3;
                        newTag4 = PersonSetter.AddedTag4;
                        newAudience = PersonSetter.AddedAudience;
                        newTopic = PersonSetter.AddedTopic;
                        newDepartment = PersonSetter.AddedDepartment;
                        break;
                }

                if (!ValidateTags) {
                    _ = await FilterHelper.ReplaceFilters(newTag1.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Tag1, SourceId = sourceId, IsActive = true }), tag1, sourceCode ?? "");
                    _ = await FilterHelper.ReplaceFilters(newTag2.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Tag2, SourceId = sourceId, IsActive = true }), tag2, sourceCode ?? "");
                    _ = await FilterHelper.ReplaceFilters(newTag3.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Tag3, SourceId = sourceId, IsActive = true }), tag3, sourceCode ?? "");
                    _ = await FilterHelper.ReplaceFilters(newTag4.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Tag4, SourceId = sourceId, IsActive = true }), tag4, sourceCode ?? "");
                    _ = await FilterHelper.ReplaceFilters(newAudience.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Audience, SourceId = sourceId, IsActive = true }), audience, sourceCode ?? "");
                    _ = await FilterHelper.ReplaceFilters(newTopic.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Topic, SourceId = sourceId, IsActive = true }), topic, sourceCode ?? "");
                    _ = await FilterHelper.ReplaceFilters(newDepartment.OrderBy(a => a).Select(a => new Tag { Title = a, TagType = TagType.Department, SourceId = sourceId, IsActive = true }), topic, sourceCode ?? "");
                }
                await Layout.AddMessage(displayString);
            }
        }
    }
}
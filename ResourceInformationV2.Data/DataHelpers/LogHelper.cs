using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.Email;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Data.DataHelpers {

    public class LogHelper(ResourceRepository resourceRepository, EmailClient emailClient, SourceEmailHelper sourceEmailHelper) {
        private readonly EmailClient _emailClient = emailClient;
        private readonly ResourceRepository _resourceRepository = resourceRepository;
        private readonly SourceEmailHelper _sourceEmailHelper = sourceEmailHelper;

        public async Task<bool> Log(CategoryType categoryType, FieldType fieldType, string netId, string sourceName, BaseObject data, string subject = "", EmailType emailOption = EmailType.None) {
            var sourceId = (await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
            if (sourceId == 0) {
                return false;
            }
            string emailString = "";
            if (emailOption != EmailType.None) {
                var emailConfiguration = await _sourceEmailHelper.GetSourceEmail(sourceName, emailOption);
                if (emailConfiguration.IsActive) {
                    if (emailConfiguration.SendToReviewEmail) {
                        emailConfiguration.Add(data.ReviewEmail);
                    }
                    emailString = await _emailClient.Send(emailConfiguration.Subject, emailConfiguration.Body, emailConfiguration.BodyText, emailConfiguration.To, emailConfiguration.Cc, emailConfiguration.ReplyTo);
                }
            }
            var log = new Log {
                Title = data.Title,
                FieldType = fieldType,
                ChangedByNetId = netId,
                SubjectId = string.IsNullOrWhiteSpace(emailString) ? subject : $"{subject} ({emailString})",
                CategoryType = categoryType,
                SourceId = sourceId,
                Data = data.ToString()
            };
            return (await _resourceRepository.CreateAsync(log)) > 0;
        }

        public async Task<bool> LogGeneric(CategoryType categoryType, string netId, string sourceName, string subject = "") {
            var sourceId = (await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
            if (sourceId == 0) {
                return false;
            }
            var log = new Log {
                Title = "",
                FieldType = FieldType.None,
                ChangedByNetId = netId,
                SubjectId = subject,
                CategoryType = categoryType,
                SourceId = sourceId,
                Data = ""
            };
            return (await _resourceRepository.CreateAsync(log)) > 0;
        }
    }
}
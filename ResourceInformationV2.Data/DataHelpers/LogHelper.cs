using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Data.DataHelpers {

    public class LogHelper {

        public async Task<bool> Log(CategoryType categoryType, FieldType fieldType, string netId, string sourceName, BaseObject data, string subject = "") {
            return true;
            /*            var sourceId = (await _programRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
                        if (sourceId == 0) {
                            return false;
                        }
                        var log = new Log {
                            Title = data.Title,
                            FieldType = fieldType,
                            ChangedByNetId = netId,
                            SubjectId = subject,
                            CategoryType = categoryType,
                            SourceId = sourceId,
                            Data = data.ToString()
                        };
                        return (await _programRepository.CreateAsync(log)) > 0;
            */
        }

        public async Task<bool> LogGeneric(CategoryType categoryType, string netId, string sourceName, string subject = "") {
            return true;
            /*            var sourceId = (await _programRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
                        if (sourceId == 0) {
                            return false;
                        }
                        var log = new Log {
                            Title = data.Title,
                            FieldType = fieldType,
                            ChangedByNetId = netId,
                            SubjectId = subject,
                            CategoryType = categoryType,
                            SourceId = sourceId,
                            Data = data.ToString()
                        };
                        return (await _programRepository.CreateAsync(log)) > 0;
            */
        }
    }
}
using ResourceInformationV2.Data.DataContext;

namespace ResourceInformationV2.Data.DataHelpers {
    public class OrgChartHelper(ResourceRepository resourceRepository) {
        private readonly ResourceRepository _resourceRepository = resourceRepository;

        public async Task<string> GetOrgChartJson(string sourceCode) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            return source?.OrgChartJson ?? "";
        }

        public async Task<bool> SaveOrgChartJson(string sourceCode, string orgChartJson) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            if (source == null) {
                return false;
            }
            source.OrgChartJson = orgChartJson;
            return await _resourceRepository.UpdateAsync(source) > 0;
        }
    }
}

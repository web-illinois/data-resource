using System.Security.Cryptography;
using System.Text;
using ResourceInformationV2.Data.DataContext;

namespace ResourceInformationV2.Data.DataHelpers {

    public class ApiHelper(ResourceRepository? resourceRepository) {
        private readonly ResourceRepository _resourceRepository = resourceRepository ?? throw new ArgumentNullException("resourceRepository");

        public async Task<string> AdvanceApi(string source) {
            var sourceItem = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == source));
            if (sourceItem == null) {
                return "";
            }
            var guid = Guid.NewGuid().ToString().ToLowerInvariant();
            var guidHash = HashWithSHA256(guid);
            sourceItem.ApiSecretPrevious = sourceItem.ApiSecretCurrent;
            sourceItem.ApiSecretCurrent = guidHash;
            sourceItem.ForceApiToDraft = true;
            sourceItem.ApiSecretLastChanged = DateTime.Now;
            return await _resourceRepository.UpdateAsync(sourceItem) > 0 ? guid : "";
        }

        public async Task<(bool allowApi, bool forceDraft)> CheckApi(string source, string key) {
            var guidHash = HashWithSHA256(key);
            var sourceItem = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == source));
            if (sourceItem == null || string.IsNullOrWhiteSpace(sourceItem.ApiSecretCurrent)) {
                return (false, false);
            }
            return (guidHash.Equals(sourceItem.ApiSecretCurrent, StringComparison.OrdinalIgnoreCase) || guidHash.Equals(sourceItem.ApiSecretPrevious, StringComparison.OrdinalIgnoreCase), sourceItem.ForceApiToDraft);
        }

        public async Task<(bool isValid, DateTime lastChanged, bool forceDraft)> GetApi(string source) {
            var sourceItem = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == source));
            return (sourceItem == null || sourceItem.ApiSecretCurrent == "") ? (false, DateTime.MinValue, false) : (true, sourceItem.ApiSecretLastChanged ?? DateTime.MinValue, sourceItem.ForceApiToDraft);
        }

        public async Task<int> InvalidateApi(string source) {
            var sourceItem = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == source));
            if (sourceItem == null) {
                return 0;
            }
            sourceItem.ApiSecretPrevious = "";
            sourceItem.ApiSecretCurrent = "";
            sourceItem.ApiSecretLastChanged = DateTime.Now;
            return await _resourceRepository.UpdateAsync(sourceItem);
        }

        public async Task<int> SetApiToDraft(string source, bool force) {
            var sourceItem = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == source));
            if (sourceItem == null) {
                return 0;
            }
            sourceItem.ForceApiToDraft = force;
            return await _resourceRepository.UpdateAsync(sourceItem);
        }

        private static string HashWithSHA256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value + "-Important-Excellent-Information-Board")));
    }
}
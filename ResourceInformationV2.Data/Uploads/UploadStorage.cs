using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ResourceInformationV2.Data.Uploads {

    public class UploadStorage {
        private readonly string _azureAccountKey = "";
        private readonly string _azureAccountName = "";
        private readonly string _azureClientUrl = "";
        private readonly string _azureImageContainerName = "";

        public UploadStorage() {
        }

        public UploadStorage(string? azureClientUrl, string? azureAccountName, string? azureAccountKey, string? azureImageContainerName) {
            _azureClientUrl = azureClientUrl ?? "";
            _azureAccountName = azureAccountName ?? "";
            _azureAccountKey = azureAccountKey ?? "";
            _azureImageContainerName = azureImageContainerName ?? "";
        }

        public async Task<bool> Delete(string url) {
            var filename = url.Split('/').Last();
            var blobServiceClient = GetServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(GetContainer());
            var blobClient = containerClient.GetBlobClient(filename);
            return await blobClient.DeleteIfExistsAsync();
        }

        public string GetFullPath(string filename) => $"{_azureClientUrl}/{GetContainer()}/{filename}";

        public bool IsPartOfPath(string url) => url.Contains(_azureClientUrl);

        public async Task<string> Move(string newFilename, string oldUrl) {
            var blobServiceClient = GetServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(GetContainer());
            var destBlobClient = containerClient.GetBlobClient(newFilename);
            _ = await destBlobClient.StartCopyFromUriAsync(new Uri(oldUrl));
            _ = await Delete(oldUrl);
            return GetFullPath(newFilename);
        }

        public async Task<string> Upload(string name, string contentType, Stream stream) {
            try {
                var filename = $"{name}{StaticLookup.SupportedImageTypes[contentType]}";
                var blobServiceClient = GetServiceClient();
                var containerClient = blobServiceClient.GetBlobContainerClient(GetContainer());
                var blobClient = containerClient.GetBlobClient(filename);
                _ = await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } });
                return filename;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return "";
            }
        }

        private string GetContainer(bool isImage = true) => _azureImageContainerName;

        private BlobServiceClient GetServiceClient() => string.IsNullOrWhiteSpace(_azureAccountName) && string.IsNullOrWhiteSpace(_azureAccountKey) ?
            new BlobServiceClient(
                new Uri(_azureClientUrl),
                new DefaultAzureCredential(true)) :
            new BlobServiceClient(new Uri(_azureClientUrl), new StorageSharedKeyCredential(_azureAccountName, _azureAccountKey));
    }
}
using Ical.Net;
using ResourceInformationV2.Search.Models;
using System.Net;

namespace ResourceInformationV2.Data.EventTranslator {
    public class IcsHolder {
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public string UploadUrl { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public List<Event> Events { get; set; } = default!;

        public async Task<bool> Load() {
            try {
                using var httpClientHandler = new HttpClientHandler {
                    CookieContainer = new CookieContainer()
                };
                using var httpClient = new HttpClient(httpClientHandler);
                var response = await httpClient.SendAsync(new HttpRequestMessage {
                    Version = HttpVersion.Version10,
                    RequestUri = new Uri(UploadUrl),
                    Method = HttpMethod.Get
                });
                _ = response.EnsureSuccessStatusCode();
                var icsContent = await response.Content.ReadAsStringAsync();
                var calendar = Calendar.Load(icsContent);
                if (calendar?.Events == null) {
                    throw new Exception("Not Found");
                }
                Events = [.. calendar.Events.Select(calendarEvent => new Event {
                    Source = Source,
                    Description = calendarEvent.Description ?? "",
                    Title = calendarEvent.Summary ?? "",
                    StartDate = calendarEvent.DtStart?.Value ?? DateTime.MinValue,
                    EndDate = calendarEvent.DtEnd?.Value ?? DateTime.MinValue,
                    Location = calendarEvent.Location ?? "",
                    CreatedOn = calendarEvent.DtStamp?.Value ?? DateTime.Now,
                    Id = Source + "-" + (calendarEvent?.Uid ?? ""),
                    IsActive = true,
                    IsAllDay = calendarEvent?.IsAllDay ?? false
                })];
            } catch (Exception e) {
                ErrorMessage = e.Message;
                return false;
            }
            return true;
        }
    }
}

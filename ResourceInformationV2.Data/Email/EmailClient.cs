using System.Text;
using Newtonsoft.Json;

namespace ResourceInformationV2.Data.Email {

    public class EmailClient(string emailApiKey, string fromEmail, string serverId, string emailUrl) {
        private readonly string _emailApiKey = emailApiKey;
        private readonly string _emailUrl = emailUrl;
        private readonly string _fromEmail = fromEmail;
        private readonly string _serverId = serverId;

        public async Task<string> Send(string subject, string body, string bodyText, string to, string cc, string replyTo) {
            try {
                if (string.IsNullOrWhiteSpace(_emailApiKey)) {
                    return "API not initalized";
                }
                if (string.IsNullOrWhiteSpace(to)) {
                    return "To value is blank";
                }

                using var clientAuth = new HttpClient();
                using var messageAuth = new HttpRequestMessage(HttpMethod.Get, _emailUrl);
                messageAuth.Headers.Add("Authorization", "Bearer " + _emailApiKey);
                messageAuth.Headers.Add("Accept", "application/json");
                var taskAuth = await clientAuth.SendAsync(messageAuth);
                if (!taskAuth.IsSuccessStatusCode) {
                    return "no authorization code";
                }
                dynamic authentication = JsonConvert.DeserializeObject(await taskAuth.Content.ReadAsStringAsync()) ?? "";
                if (authentication.data == null || authentication.data.apiKey == null || authentication.data.gateway == null) {
                    return "invalid authorization code";
                }
                string injectionApi = authentication.data.apiKey.ToString();
                string injectionUrl = authentication.data.gateway.ToString();

                if (string.IsNullOrWhiteSpace(injectionUrl) || string.IsNullOrWhiteSpace(injectionApi)) {
                    return "no authorization code from successful credential call (url is '" + injectionUrl + "')";
                }

                var json = "{\"serverId\": " + _serverId + ", \"APIKey\": \"" + injectionApi + "\", \"Messages\": [ { \"To\": [ \"" + CreateAddress(to.Split(';')) + "\" ], \"Cc\": [ \"" + CreateAddress(cc.Split(';')) + "\" ], \"From\": { \"emailAddress\": \"" + _fromEmail + "\" }, \"ReplyTo\": { \"emailAddress\": \"" + replyTo + "\" }, \"Subject\": \"" + CleanText(subject) + "\", \"TextBody\": \"" + CleanText(bodyText) + "\", \"HtmlBody\": \"" + CleanText(body) + "\" } ] }";

                using var client = new HttpClient();
                using var message = new HttpRequestMessage(HttpMethod.Post, injectionUrl);
                message.Content = new StringContent(json, Encoding.UTF8, "application/json");
                message.Headers.Add("Accept", "application/json");
                var task = await client.SendAsync(message);
                return task.IsSuccessStatusCode ? "Email sent" : await task.Content.ReadAsStringAsync() + ": " + json;
            } catch (Exception e) {
                return e.ToString();
            }
        }

        private static string CleanText(string s) => s.Replace("\"", "");

        private static string CreateAddress(IEnumerable<string> list) => string.Join(", ", list.Select(l => "{ \"emailAddress\" : \"" + l + "\" }"));
    }
}
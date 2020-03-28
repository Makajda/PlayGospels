using Prism.Events;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlayGospels.Common {
    public class Utilities {
        private static readonly HttpClient client = new HttpClient();
        public static async Task LoadToFile(string fromAddress, string toAddress, CancellationToken cancellationToken) {
            using (HttpResponseMessage response = await client.GetAsync(fromAddress)) {
                response.EnsureSuccessStatusCode();
                using (Stream stream = await response.Content.ReadAsStreamAsync()) {
                    using (var fileStream = new FileStream(toAddress, FileMode.Create, FileAccess.Write, FileShare.None)) {
                        await stream.CopyToAsync(fileStream, 81920, cancellationToken);
                    }
                }
            }
        }
    }

    public class AppExitEvent : PubSubEvent { }
    public class LabelJust : Label { }
}

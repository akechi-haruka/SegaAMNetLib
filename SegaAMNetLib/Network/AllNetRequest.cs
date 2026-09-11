using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using Haruka.Arcade.SegaAMNetLib.Util;
using Haruka.Common;
using Microsoft.Extensions.Logging;

namespace Haruka.Arcade.SegaAMNetLib.Network;

public class AllNetRequest {
    protected static readonly ILogger LOG = Log.GetOrCreate("Allnet");

    private readonly string url;
    private readonly Dictionary<string, string> parameters;
    private readonly HttpClient client;

    public AllNetRequest(String url, Dictionary<string, string> parameters) {
        this.url = url;
        this.parameters = parameters;
        client = new HttpClient();
    }

    public virtual NameValueCollection Execute() {
        try {
            AssemblyName version = Assembly.GetExecutingAssembly().GetName();
            Uri uri = new Uri(url);

            LOG.LogInformation("Sending network request to " + uri);

            HttpResponseMessage resp = client.Send(new HttpRequestMessage() {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new ByteArrayContent(Dfi.Encode(String.Join("&", parameters.Select(kvp =>
                    $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value.ToString())}")))) {
                    Headers = {
                        ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                    }
                },
                Headers = {
                    ConnectionClose = true,
                    Pragma = {
                        new NameValueHeaderValue("Pragma", "DFI")
                    },
                    UserAgent = {
                        new ProductInfoHeaderValue(version.Name, version.Version.ToString())
                    },
                    Host = uri.Host
                }
            });

            if (resp.StatusCode != HttpStatusCode.OK) {
                LOG.LogDebug("Response: " + resp.Content.ReadString());
                throw new Exception("Bad status code: " + resp.StatusCode);
            }

            String content = resp.Headers.Pragma.Select(h => h.Value).Any(v => v == "DFI") ? Dfi.Decode(resp.Content.ReadBytes()) : resp.Content.ReadString();

            LOG.LogDebug("Response: " + content);

            return HttpUtility.ParseQueryString(content);
        } catch (Exception ex) {
            LOG.LogError(ex, "All.Net request to " + url + " failed");
            throw;
        }
    }
}
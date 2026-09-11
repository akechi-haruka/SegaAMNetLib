using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using Haruka.Arcade.SegaAMNetLib.Util;
using Haruka.Common;
using Microsoft.Extensions.Logging;

namespace Haruka.Arcade.SegaAMNetLib.Network;

public class WebRequest {
    protected static readonly ILogger LOG = Log.GetOrCreate("Web");

    private readonly string url;
    private readonly StringContent requestContent;
    private readonly HttpClient client;

    public WebRequest(String url, StringContent requestContent) {
        this.url = url;
        this.requestContent = requestContent;
        client = new HttpClient();
    }

    public virtual string Execute() {
        try {
            AssemblyName version = Assembly.GetExecutingAssembly().GetName();
            Uri uri = new Uri(url);

            LOG.LogInformation("Sending network request to " + uri);

            HttpResponseMessage resp = client.Send(new HttpRequestMessage() {
                RequestUri = uri,
                Content = requestContent,
                Headers = {
                    ConnectionClose = true,
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

            String content = resp.Content.ReadString();

            LOG.LogDebug("Response: " + content);

            return content;
        } catch (Exception ex) {
            LOG.LogError(ex, "Web request to " + url + " failed");
            throw;
        }
    }
}
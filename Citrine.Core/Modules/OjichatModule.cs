using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Citrine.Core.Api;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
    public class OjichatModule : ModuleBase
    {
        public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
        {
            if (n.Text is string text && text.Contains("おじさん"))
            {
                var req = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("name", n.User.ScreenName ?? n.User.Name)
                });
                var res = await (await cli.PostAsync("https://ojichat.appspot.com/post", req)).Content.ReadAsStringAsync();
                await shell.ReplyAsync(n, JsonConvert.DeserializeObject<OjichatResponse>(res).Message);
                return true;
            }
            return false;
        }

        class OjichatResponse
        {
            [JsonProperty("message")]
            public string Message { get; set; }
        }

        HttpClient cli = new HttpClient();
    }
}

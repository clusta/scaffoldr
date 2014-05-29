using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace ScaffoldR.Providers
{
    public class RemoteTextTemplate : ITextTemplate
    {
        private string uri;
        
        public string RenderTemplate(object page)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResponse = httpClient.PostAsJsonAsync(uri, page).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    return httpResponse.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public RemoteTextTemplate(string uri)
        {
            this.uri = uri;
        }
    }
}

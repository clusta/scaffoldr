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
        private string path;
        
        public string RenderTemplate(object page)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResponse = httpClient.PostAsJsonAsync(path, page).Result;

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

        public RemoteTextTemplate(string path)
        {
            this.path = path;
        }
    }
}

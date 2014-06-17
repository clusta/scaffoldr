using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR
{
    public class Output
    {
        [JsonProperty("base_address")]
        public string BaseAddress { get; set; }

        [JsonProperty("bucket_name")]
        public string BucketName { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [JsonProperty("secret_key")]
        public string SecretKey { get; set; }

        [JsonProperty("content_path")]
        public string ContentPath { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("file_extension")]
        public string FileExtension { get; set; }
    }
}

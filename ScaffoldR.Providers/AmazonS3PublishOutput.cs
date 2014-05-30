using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldR.Providers
{
    public class AmazonS3PublishOuput : IPublishOutput
    {
        private string accessKey;
        private string secretKey;
        private string bucketName;
        private RegionEndpoint region;

        public async Task SaveAsync(Stream inputStream, string path, string contentType)
        {
            inputStream.Position = 0;

            using (var client = AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, region))
            {
                var transferUtility = new TransferUtility(client);
                var transferRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = path,
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = inputStream,
                    ContentType = contentType
                };

                await transferUtility.UploadAsync(transferRequest);
            }
        }

        public AmazonS3PublishOuput(string accessKey, string secretKey, string bucketName, RegionEndpoint region = null)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.bucketName = bucketName;
            this.region = region ?? RegionEndpoint.EUWest1;
        }
    }
}

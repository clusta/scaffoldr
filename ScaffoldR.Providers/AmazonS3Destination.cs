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
    public class AmazonS3Destination : IFileDestination
    {
        private string accessKey;
        private string secretKey;
        private string bucketName;

        public async Task SaveAsync(string path, string contentType, Stream inputStream)
        {
            inputStream.Seek(0, SeekOrigin.Begin);

            using (var client = AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey))
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

        public AmazonS3Destination(string accessKey, string secretKey, string bucketName)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
            this.bucketName = bucketName;
        }
    }
}

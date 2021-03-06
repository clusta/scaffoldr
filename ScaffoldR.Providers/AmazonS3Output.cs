﻿using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;

namespace ScaffoldR.Providers
{
    public class AmazonS3Output : IFileOutput
    {
        private string serviceUrl;
        private string accessKey;
        private string secretKey;
        private string bucketName;

        public async Task SaveAsync(string path, string contentType, Stream inputStream)
        {
            inputStream.Seek(0, SeekOrigin.Begin);

            var config = new AmazonS3Config()
            {
                ServiceURL = serviceUrl
            };

            using (var client = AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, config))
            {
                var transferUtility = new TransferUtility(client);
                var transferRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = path,
                    CannedACL = S3CannedACL.PublicRead,
                    InputStream = inputStream,
                    ContentType = contentType,
                };

                await transferUtility.UploadAsync(transferRequest);
            }
        }

        public AmazonS3Output(string serviceUrl, string bucketName, string accessKey, string secretKey)
        {
            Requires.NotNull(serviceUrl, "serviceUrl");
            Requires.NotNull(bucketName, "bucketName");
            Requires.NotNull(accessKey, "accessKey");
            Requires.NotNull(secretKey, "secretKey");
            
            this.serviceUrl = serviceUrl;
            this.bucketName = bucketName;
            this.accessKey = accessKey;
            this.secretKey = secretKey;
        }
    }
}

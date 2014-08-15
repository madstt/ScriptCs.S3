using System;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ScriptCs.Contracts;
using ScriptCs.Rebus;

namespace ScriptCs.S3
{
    public class S3Script : IScriptPackContext
    {
        private string _bucket;
        private TransferUtility _transferUtility;
        private static int _percentComplete;

        public S3Script ConfigureWith(string bucket)
        {
            Guard.AgainstNullArgument("bucket", bucket);

            _bucket = bucket;
            _transferUtility = new TransferUtility("", "", RegionEndpoint.GetBySystemName("us-east-1"));


            return this;
        }

        public void UploadFile()
        {
            VerifyBucketExistence();

            var request = new TransferUtilityUploadRequest
            {
                BucketName = _bucket,
                FilePath = "c:\\images\\IMG_6519.JPG"
            };

            Console.WriteLine("Key: " + request.Key);

            //var fileInfo =
            //    _transferUtility.S3Client.GetObjectMetadata(new GetObjectMetadataRequest
            //    {
            //        BucketName = _bucket,
            //        Key = "IMG_6519.JPG"
            //    });
            
            //Console.WriteLine(fileInfo.LastModified);
            try
            {
                //Console.WriteLine("Exists: " + fileInfo.Exists);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error encountered: " +  e.Message);
            }

            //Console.Write("Copying {0}:\n ", request.FilePath);
            request.UploadProgressEvent +=
                (sender, args) => Console.Write("\rCopying {0}: [{1} ", request.FilePath, ProgressBar(args.PercentDone));

            _transferUtility.Upload(request);
            Console.Write("] - done.\n");

        }

        public void UploadDirectory(string directoryPath)
        {
            VerifyBucketExistence();

            var request = new TransferUtilityUploadDirectoryRequest()
            {
                BucketName = _bucket,
                Directory = directoryPath,
            };

            request.UploadDirectoryProgressEvent +=
                (sender, args) => Console.Write("\rCopying {0}: [{1} ", request.Directory, ProgressBar((args.NumberOfFilesUploaded / args.TotalNumberOfFiles) * 100));

            _transferUtility.UploadDirectory(request);
            Console.Write("] - done.\n");

        }

        private void VerifyBucketExistence()
        {
            try
            {
                _transferUtility.S3Client.PutBucket(new PutBucketRequest { BucketName = _bucket });
            }
            catch (WebException e)
            {
                Console.WriteLine("Error while connecting...");

                throw;
            }
        }

        private string ProgressBar(long percentDone)
        {
            var progrss = string.Empty;
            for (int i = 0; i < percentDone; i++)
            {
                progrss += "|";
            }
            return progrss;
        }
    }
}
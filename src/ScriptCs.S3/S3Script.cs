using System;
using Amazon;
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
            _transferUtility = new TransferUtility("AKIAID7YP6HFINEK3SMQ", "tyNACmmYQc4lXYReWU1fkv7X2BISBAF2FHsKStxd", RegionEndpoint.GetBySystemName("us-east-1"));

            return this;
        }

        public void Upload()
        {
            VerifyBucketExistence();

            var request = new TransferUtilityUploadDirectoryRequest
            {
                BucketName = _bucket,
                Directory = "c:\\images"
            };

            //Console.Write("Copying {0}:\n ", request.FilePath);
            request.UploadDirectoryProgressEvent +=
                //(sender, args) => Console.Write("\rCopying {0}: {1}% ", request.FilePath, args.PercentDone);
                (sender, args) => Console.Write("\rCopying {0}: [{1} ", request.Directory, ProgressBar((args.TotalNumberOfFiles / args.NumberOfFilesUploaded) * 100));

            _transferUtility.UploadDirectory(request);
            Console.Write("] - done.\n");

        }

        private void VerifyBucketExistence()
        {
            _transferUtility.S3Client.PutBucket(new PutBucketRequest {BucketName = _bucket});
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
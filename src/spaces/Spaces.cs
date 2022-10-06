using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime.CredentialManagement.Internal;
using Amazon.Util;

namespace DigitalOceanSpaces;

public partial class Spaces
{
    public IAmazonS3 S3Client { get; set; }

    public Spaces(string keyId, string secret, string endpoint)
    {
        /*
        By default, the shared AWS credentials file is located in the .aws directory within your home directory and is named credentials; that is, ~/.aws/credentials (Linux or macOS) or %USERPROFILE%\.aws\credentials (Windows).
        */
        var options = new CredentialProfileOptions { AccessKey = keyId, SecretKey = secret };

        var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);

        AmazonS3Config amazonS3Config = new ()
        {
            ServiceURL = endpoint,
            Timeout = TimeSpan.FromSeconds(value: 10),
            RetryMode = RequestRetryMode.Standard,
            MaxErrorRetry = 3
        };
        
        S3Client = new AmazonS3Client(credentials: credentials, clientConfig: amazonS3Config);
    }

    public async Task<List<string>?> ListBucketsAsync()
    {
        try
        {
            var buckets = await this.S3Client.ListBucketsAsync();
            return buckets.Buckets.Select(selector: c => c.BucketName).ToList();
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                format: "Error encountered ***. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
    }

    public async Task<bool> DoesBucketExistAsync(string bucket) => await this.S3Client.DoesS3BucketExistAsync(bucketName: bucket);

    public async Task<List<string?>?> GetAllObjectKeysAsync(string bucket, string? prefix = null)
    {
        try
        {
            var response = await this.S3Client.ListObjectsAsync(bucketName: bucket, prefix: prefix);
            return response.S3Objects.Select(selector: c => c?.Key).ToList();
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                format: "Error encountered ***. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
    }

    public async Task<Stream?> GetObjectStreamAsync(string bucket, string key)
    {
        try
        {
            var response = await this.S3Client.GetObjectAsync(bucketName: bucket, key: key);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                format: "Error encountered ***. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
    }

    public async Task<HttpStatusCode> UploadObjectAsync(string bucketName, string key, string filepath, string contentType = "image/jpg")
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                FilePath = filepath,
                ContentType = contentType
            };

            PutObjectResponse response = await S3Client.PutObjectAsync(request: putRequest);
            return response.HttpStatusCode;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                    format: "Error encountered ***. Message:'{0}' when writing an object"
                    , arg0: e.Message);
            return HttpStatusCode.BadRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return HttpStatusCode.BadRequest;
        }
    }

    public async Task<HttpStatusCode> DeleteObjectsAsync(string bucketName, List<string> keys)
    {
        try
        {
            List<KeyVersion> versions = new List<KeyVersion>();
            foreach (string key in keys)
            {
                var keyVersion = new KeyVersion() { Key = key };
                versions.Add(item: keyVersion);
            }

            var multiObjectRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = versions
            };
            DeleteObjectsResponse delObjRes = await S3Client.DeleteObjectsAsync(request: multiObjectRequest);
            return delObjRes.HttpStatusCode;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                format: "Error encountered ***. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return HttpStatusCode.BadRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return HttpStatusCode.BadRequest;
        }
    }

    public async Task<HttpStatusCode> DeleteObjectAsync(string bucketName, string key)
    {
        try
        {
            var DeleteObjectRequest = new DeleteObjectRequest { BucketName = bucketName, Key = key };
            DeleteObjectResponse delObjRes = await S3Client.DeleteObjectAsync(request: DeleteObjectRequest);
            return delObjRes.HttpStatusCode;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                format: "Error encountered ***. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return HttpStatusCode.BadRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return HttpStatusCode.BadRequest;
        }
    }

    public string? GetObjectUrl(string bucketName, string key, double durationInSeconds = 30)
    {
        try
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddSeconds(value: durationInSeconds)
            };
            return S3Client.GetPreSignedURL(request: request);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                format: "Error encountered ***. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                format: "Unknown encountered on server. Message:'{0}' when writing an object"
                , arg0: e.Message);
            return null;
        }
    }
}
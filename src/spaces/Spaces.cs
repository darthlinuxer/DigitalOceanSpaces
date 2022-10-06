namespace DigitalOceanSpaces;

public partial class Spaces
{
    public IAmazonS3 S3Client { get; set; }

    public Spaces(string keyId, string secret, string endpoint, string region = "us-east-1", string profileName = "default")
    {
        /*
        By default, the shared AWS credentials file is located in the .aws directory within your home directory and is named credentials; that is, ~/.aws/credentials (Linux or macOS) or %USERPROFILE%\.aws\credentials (Windows).
        */
        var sharedFile = new SharedCredentialsFile();

        if (!sharedFile.TryGetProfile(profileName, out CredentialProfile profile))
        {
            sharedFile.UnregisterProfile(profileName);
        }

        var amazonRegion = RegionEndpoint.GetBySystemName(region);
        WriteProfile(profileName, keyId, secret, amazonRegion ?? RegionEndpoint.USEast1);

        AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedFile, out AWSCredentials credentials);
        AmazonS3Config amazonS3Config = new()
        {
            ServiceURL = endpoint,
            Timeout = TimeSpan.FromSeconds(10),
            RetryMode = RequestRetryMode.Standard,
            MaxErrorRetry = 3
        };
        S3Client = new AmazonS3Client(credentials, amazonS3Config);
    }

    public async Task<List<string>?> ListBucketsAsync()
    {
        try
        {
            var buckets = await this.S3Client.ListBucketsAsync();
            return buckets.Buckets.Select(c => c.BucketName).ToList();
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                "Error encountered ***. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
    }

    public async Task<bool> DoesBucketExistAsync(string bucket) => await this.S3Client.DoesS3BucketExistAsync(bucket);

    public async Task<List<string?>?> GetAllObjectKeysAsync(string bucket, string? prefix = null)
    {
        try
        {
            var response = await this.S3Client.ListObjectsAsync(bucket, prefix);
            return response.S3Objects.Select(c => c?.Key).ToList();
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                "Error encountered ***. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
    }

    public async Task<Stream?> GetObjectStreamAsync(string bucket, string key)
    {
        try
        {
            var response = await this.S3Client.GetObjectAsync(bucket, key);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                "Error encountered ***. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
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

            PutObjectResponse response = await S3Client.PutObjectAsync(putRequest);
            return response.HttpStatusCode;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                    "Error encountered ***. Message:'{0}' when writing an object"
                    , e.Message);
            return HttpStatusCode.BadRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
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
                versions.Add(keyVersion);
            }

            var multiObjectRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = versions
            };
            DeleteObjectsResponse delObjRes = await S3Client.DeleteObjectsAsync(multiObjectRequest);
            return delObjRes.HttpStatusCode;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                "Error encountered ***. Message:'{0}' when writing an object"
                , e.Message);
            return HttpStatusCode.BadRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
            return HttpStatusCode.BadRequest;
        }
    }

    public async Task<HttpStatusCode> DeleteObjectAsync(string bucketName, string key)
    {
        try
        {
            var DeleteObjectRequest = new DeleteObjectRequest { BucketName = bucketName, Key = key };
            DeleteObjectResponse delObjRes = await S3Client.DeleteObjectAsync(DeleteObjectRequest);
            return delObjRes.HttpStatusCode;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                "Error encountered ***. Message:'{0}' when writing an object"
                , e.Message);
            return HttpStatusCode.BadRequest;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
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
                Expires = DateTime.UtcNow.AddSeconds(durationInSeconds)
            };
            return S3Client.GetPreSignedURL(request);
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine(
                "Error encountered ***. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                "Unknown encountered on server. Message:'{0}' when writing an object"
                , e.Message);
            return null;
        }
    }
}
using Amazon.S3;
using Amazon.S3.Model;

namespace DigitalOceanSpaces;
public class Spaces
{
    public Spaces()
    {

    }

    public static async Task<ListBucketsResponse> GetBuckets(IAmazonS3 client)
    {
        return await client.ListBucketsAsync();
    }
}

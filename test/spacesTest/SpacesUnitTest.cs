using DigitalOceanSpaces;
using System.Net;

namespace SpacesTest;

//https://github.com/awsdocs/aws-doc-sdk-examples/tree/main/dotnetv3/S3
[TestClass]
public class SpacesUnitTest
{
    private readonly Spaces spaces;

    public SpacesUnitTest()
    {
        var endpoint = "https://nyc3.digitaloceanspaces.com";
        var key = "yourKey";
        var secret = "yourSecret";
        spaces = new Spaces(keyId: key, secret: secret, endpoint: endpoint);
    }

    [TestMethod]
    public async Task ListBucketsAsync()
    {
        var result = await spaces.ListBucketsAsync();
        Assert.IsTrue(condition: result.Count() > 0);
    }

    [TestMethod]
    public async Task GetAllObjectKeysAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var result = await spaces.GetAllObjectKeysAsync(bucket: bucket);
        Assert.IsTrue(condition: result is not null);
    }

    [TestMethod]
    public async Task GetObjectStreamAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var keys = await spaces.GetAllObjectKeysAsync(bucket: bucket);
        var aKey = keys?.Skip(count: 2).Take(count: 1).First();
        using var objStream = await spaces.GetObjectStreamAsync(bucket: bucket, key: aKey);
        Spaces.SaveStreamAsFile(filePath: "c:/test", inputStream: objStream, fileName: "teste.jpg");
        Assert.IsTrue(condition: File.Exists(path: "c:/test/teste.jpg"));
    }

    [TestMethod]
    public async Task UploadObjectAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var result = await spaces.UploadObjectAsync(bucketName: bucket, key: "guiatijucano/teste", filepath: "c:/test/teste.jpg");
        Assert.IsTrue(condition: result == HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task DeleteObjectAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var result = await spaces.DeleteObjectAsync(bucketName: bucket, key: "guiatijucano/teste");
        Assert.IsTrue(condition: result == HttpStatusCode.NoContent);
    }

    [TestMethod]
    public void GetObjectUrl()
    {
        var result = spaces.GetObjectUrl(bucketName: "offers", key: "guiatijucano/teste", durationInSeconds: 30);
        Assert.IsTrue(condition: result is not null);
    }
}
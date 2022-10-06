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
        spaces = new Spaces(key, secret, endpoint, "us-east-1", "default");
    }

    [TestMethod]
    public async Task ListBucketsAsync()
    {
        var result = await spaces.ListBucketsAsync();
        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task GetAllObjectKeysAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var result = await spaces.GetAllObjectKeysAsync(bucket);
        Assert.IsTrue(result is not null);
    }

    [TestMethod]
    public async Task GetObjectStreamAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var keys = await spaces.GetAllObjectKeysAsync(bucket);
        var aKey = keys?.Skip(2).Take(1).First();
        using var objStream = await spaces.GetObjectStreamAsync(bucket, aKey);
        Spaces.SaveStreamAsFile("c:/test", objStream, "teste.jpg");
        Assert.IsTrue(File.Exists("c:/test/teste.jpg"));
    }

    [TestMethod]
    public async Task UploadObjectAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var result = await spaces.UploadObjectAsync(bucket, "guiatijucano/teste", "c:/test/teste.jpg");
        Assert.IsTrue(result == HttpStatusCode.OK);
    }

    [TestMethod]
    public async Task DeleteObjectAsync()
    {
        var bucket = (await spaces.ListBucketsAsync())?.First();
        var result = await spaces.DeleteObjectAsync(bucket, "guiatijucano/teste");
        Assert.IsTrue(result == HttpStatusCode.NoContent);
    }

    [TestMethod]
    public void GetObjectUrl()
    {
        var result = spaces.GetObjectUrl("offers", "guiatijucano/teste", 30);
        Assert.IsTrue(result is not null);
    }
}
namespace DigitalOceanSpaces;

public partial class Spaces
{
    private static void WriteProfile(string profileName, string keyId, string secret, RegionEndpoint region)
    {
        Console.WriteLine($"Create the [{profileName}] profile...");
        var options = new CredentialProfileOptions { AccessKey = keyId, SecretKey = secret };
        var profile = new CredentialProfile(profileName, options)
        {
            Region = region,
            MaxAttempts = 3
        };

        var sharedFile = new SharedCredentialsFile();
        sharedFile.RegisterProfile(profile);
    }

    public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
    {
        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        string path = Path.Combine(filePath, fileName);
        using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
        {
            inputStream.CopyTo(outputFileStream);
        }
    }
}
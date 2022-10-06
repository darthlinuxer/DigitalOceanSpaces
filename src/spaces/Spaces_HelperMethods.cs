namespace DigitalOceanSpaces;

public partial class Spaces
{
    private static void WriteProfile(string profileName, string keyId, string secret, RegionEndpoint region)
    {
        Console.WriteLine(value: $"Create the [{profileName}] profile...");
        var options = new CredentialProfileOptions { AccessKey = keyId, SecretKey = secret };
        var profile = new CredentialProfile(name: profileName, profileOptions: options)
        {
            Region = region,
            MaxAttempts = 3
        };

        var sharedFile = new SharedCredentialsFile();
        sharedFile.RegisterProfile(profile: profile);
    }

    public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
    {
        DirectoryInfo info = new DirectoryInfo(path: filePath);
        if (!info.Exists)
        {
            info.Create();
        }

        string path = Path.Combine(path1: filePath, path2: fileName);
        using (FileStream outputFileStream = new FileStream(path: path, mode: FileMode.Create))
        {
            inputStream.CopyTo(destination: outputFileStream);
        }
    }
}
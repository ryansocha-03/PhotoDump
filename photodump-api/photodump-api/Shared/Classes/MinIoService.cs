using photodump_api.Shared.Interfaces;

namespace photodump_api.Shared.Classes;

public class MinIoService: IMediaStorage
{
    public Task<Uri> RequestUpload()
    {
        var tempUri = new Uri("https://photodump-api.min.io/upload");
        return Task.FromResult(tempUri);
    }

    public Task<Uri> RequestDownload()
    {
        var tempUri = new Uri("https://photodump-api.min.io/download");
        return Task.FromResult(tempUri);
    }
}
namespace photodump_api.Shared.Interfaces;

public interface IMediaStorage
{
    public Task<Uri> RequestUpload();
    
    public Task<Uri> RequestDownload();
}
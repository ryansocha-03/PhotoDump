using Minio;

namespace ContentStore.MinIO.Wrappers;

public interface IExternalS3Client 
{
    IMinioClient MinioClient { get; }
}
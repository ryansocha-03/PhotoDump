using Minio;

namespace ContentStore.MinIO.Interfaces;

public interface IExternalS3Client 
{
    IMinioClient MinioClient { get; }
}
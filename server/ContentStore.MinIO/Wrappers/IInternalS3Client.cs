using Minio;

namespace ContentStore.MinIO.Interfaces;

public interface IInternalS3Client
{
    IMinioClient MinioClient { get; }
}
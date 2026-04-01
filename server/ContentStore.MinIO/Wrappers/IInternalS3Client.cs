using Minio;

namespace ContentStore.MinIO.Wrappers;

public interface IInternalS3Client
{
    IMinioClient MinioClient { get; }
}
using Minio;

namespace ContentStore.MinIO.Interfaces;

public class InternalS3Client(IMinioClient minioClient) : IInternalS3Client
{
    public IMinioClient MinioClient { get; } = minioClient;
}
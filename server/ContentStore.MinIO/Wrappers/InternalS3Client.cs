using Minio;

namespace ContentStore.MinIO.Wrappers;

public class InternalS3Client(IMinioClient minioClient) : IInternalS3Client
{
    public IMinioClient MinioClient { get; } = minioClient;
}
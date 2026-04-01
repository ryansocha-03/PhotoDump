using Minio;

namespace ContentStore.MinIO.Wrappers;

public class ExternalS3Client(IMinioClient minioClient) : IExternalS3Client 
{
    public IMinioClient MinioClient { get; } = minioClient;
}
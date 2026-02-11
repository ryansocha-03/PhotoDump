namespace Core.Models;

public static class UploadStatus
{
    public static readonly string Pending = "pending";
    public static readonly string Uploaded = "uploaded";
    public static readonly string Processing = "processing";
    public static readonly string Completed = "completed";
    public static readonly string Failed = "failed";
    public static readonly string Cancelled = "cancelled";
    public static readonly string Deleted = "deleted";
}
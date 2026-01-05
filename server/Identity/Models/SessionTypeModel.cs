namespace Identity.Models;

public class SessionTypeModel
{
    public required SessionType SessionType { get; set; }
}

public enum SessionType
{
    Anonymous,
    Guest
}


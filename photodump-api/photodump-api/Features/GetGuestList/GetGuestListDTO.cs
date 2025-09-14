namespace photodump_api.Features.GetGuestList;

public class GetGuestListDto
{
    public required int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
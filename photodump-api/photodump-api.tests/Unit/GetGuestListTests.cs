using Moq;
using photodump_api.Domain;
using photodump_api.Features.GetGuestList;
using photodump_api.Infrastructure.Db.Repositories;

namespace photodump_api.tests.Unit;

public class GetGuestListTests
{
    [Fact]
    public async Task GetAllGuests_HasGuests_WhenEventExists()
    {
        var eventId = 1;

        var guest1 = new Guest { FirstName = "Amanda", LastName = "Socha", EventId = eventId};
        List<Guest> sampleGuesList = new List<Guest>()
        {
            guest1
        };
        
        // Arrange
        var guestMock = new Mock<IGuestRepository>();

        guestMock.Setup(g => g.GetAllAsync(eventId))
            .ReturnsAsync(sampleGuesList);

        var handler = new GetGuestListHandler(guestMock.Object);

        // Act
        var result = await handler.HandleAsync(eventId);
        
        // Assert
        Assert.Equal(sampleGuesList.Count, result.Count);
        Assert.Equal(sampleGuesList.First().FirstName, result.First().FirstName);
        guestMock.Verify(g => g.GetAllAsync(eventId), Times.Once);
    }
}
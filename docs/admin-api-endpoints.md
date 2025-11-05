# Event Admin API Endpoints (planning only)

## Events
POST /events
- Creating a new event
- Returns: the event object

GET /events/{event id}
- Gets details of a specific event
- Returns: the event object

PATCH /events/{event id}
- Upates various fields of an event object
- Returns: The updated event object

## Guests
GET /events/{event id}/guests
- Gets full guest list for an event
- Returns: List of guest objects

GET /events/{event id}/guests/{guest id}
- Gets details on a specific guest
- Returns: The guest object

POST /events/{event id}/guests
- Creates a new guest
- Payload: Required info about a guest
- Returns: The guest object

DELETE /events/{event id}/guests/{guest id}
- Deletes the specified guest
- Returns: The guest object

PATCH /events/{event id}/guests/{guest id}
- Updates various fields of the guest object
- Returns: The updated guest object

## Media
GET /events/{event id}/media
- Initiates a bulk download of all event media. Sends to admin via email
- Returns: Accepted
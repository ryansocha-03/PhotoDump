# Application API Endpoints - Prototype (planning only)

## Levels of access
- public
- anonymous guest: someone who has entered an event password but not a guest
- event guest: someone who has entered an event password and a valid guest

## Events
POST /events/verify-password
- For when a user submits a password and we want to see if it corresponds to a valid event or not
- Payload: the user entered password
- Returns: On success, sets token for anonymous guest. Failure in body otherwise (always returns 200)
- Access: public

## Guests
GET /events/{event id}/guests/search?query={user query}
- Getting top N guest suggestions from user input
- Returns: List of guest objects
- Access: anonymous event guest

POST /events/{event id}/guests/{guest id}/token
- Exchanges anonymous guest token for verified guest token
- Payload: the guest that was selected
- Returns: On success, exchanges anonymous guest token with verified guest token
- Access: anonymous event guest


## Media Meta
GET /events/{event id}/guests/{guest id}/media
- Gets all media uploaded by a particular guest of a particular event
- Returns: A list of media objects
- Access: event guest

GET /events/{event id}/media
- Gets all public media for a specific event 
- Returns: A list of media objects
- Access: event guest

POST /events/{event id}/guests/{guest id}/media/delete
- Deletes the provided media
- Payload: a list of media IDs to delete
- Access: event guest

POST /events/{event id}/guests/{guest id}/media/batch
- Updates a batch of media. Used for toggling privacy
- Payload: JSON specifying IDs and field to update
- Access: event guest

## Media Content
### General Download Behavior
When a user requests to download some media, for requests of <= 25 media, one URL per media is generated, and the user downloads each individually.

For requests of > 25 media, a zip file is created on media store, and a single signed URL is created. The zip is then downloaded to the client.

In either case, the API returns a list of signed URLs (even in the bulk download case)

POST /events/{event id}/guests/{guest id}/media/download
- Generates signed download URLs for when a guest wants to download their media
- Payload: A list of media IDs to download
- Returns: A list of signed URLs (? maybe have to return polling data if takes a while)
- Access: event guest

POST /events/{event id}/media/download
- Generates signed download URLs for when a guest wants to download event media
- Payload: A list of media IDs to download
- Returns: A list of signed URLs (? maybe have to return polling data if takes a while)
- Access: event guest

POST /events/{event id}/media/upload
- Takes metadata for new media to upload and returns signed upload URLs to the client can upload their media
- Payload: A list of media objects
- Returns: A signed upload URL (? might have to return polling data if takes a while)
- Access: event guest


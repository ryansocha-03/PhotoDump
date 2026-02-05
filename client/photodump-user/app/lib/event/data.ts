import { ApiResponseModel } from "../types";
import { EventLandingData } from "./types";
import { EVENT_HEADER_NAME } from "../auth/cookie";

export async function getEventLandingData(eventPublicId: string) : Promise<ApiResponseModel<EventLandingData>> {
   let eventData: ApiResponseModel<EventLandingData> = {
        code: 200,
        data: null
    }
    
    let eventDataResponse: Response;
    try {
        const eventDataRequest = new Request(`${process.env.APP_API_URL}/events/landing`);
        eventDataRequest.headers.append(EVENT_HEADER_NAME, eventPublicId);
        eventDataResponse = await fetch(eventDataRequest);
    }
    catch (e) {
        eventData.code = 500;
        return eventData;
    }

    eventData.code = eventDataResponse.status; 
    if (!eventDataResponse.ok) {
        return eventData;
    }

    const eventDataJson: EventLandingData = await eventDataResponse.json();
    eventData.data = eventDataJson;

    return eventData;
}
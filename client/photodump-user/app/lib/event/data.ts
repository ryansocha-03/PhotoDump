import { ApiResponseModel } from "../types";
import { EventLandingData } from "./types";

export async function getEventLandingData(eventPublicId: string) : Promise<ApiResponseModel<EventLandingData>> {
   let eventData: ApiResponseModel<EventLandingData> = {
        code: 200,
        data: null
    }
    
    let eventDataResponse: Response;
    try {
        eventDataResponse = await fetch(`${process.env.APP_API_URL}/api/events/${eventPublicId}`);
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
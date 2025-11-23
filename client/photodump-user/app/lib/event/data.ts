import { mockEventLanding } from "@/app/mock-data/event-mock-data";
import { ApiResponseModel } from "../types";
import { EventLandingData } from "./types";

export async function getEventLandingData(eventPublicId: string) : Promise<ApiResponseModel<EventLandingData>> {
   let eventData: ApiResponseModel<EventLandingData> = {
        code: 200,
        data: null
    }
    
    if (process.env.APP_ENVIRONMENT == 'local') {
        let mockData = mockEventLanding;
        mockData.eventPublicId = eventPublicId;
        eventData.data = mockData;
        return eventData;
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
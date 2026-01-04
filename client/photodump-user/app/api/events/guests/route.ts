import { addEventHeaders } from "@/app/lib/auth/api";
import { getSessionFromCookie } from "@/app/lib/auth/cookie";
import { EventGuest } from "@/app/lib/event/types";
import { mockGuestListSearch } from "@/app/mock-data/event-mock-data";
import { NextRequest, NextResponse } from "next/server";

export interface GuestSearchSubmission {
    eventId: string,
    guestName: string
}

export async function POST(request: NextRequest) : Promise<NextResponse> {
    const requestData: GuestSearchSubmission = await request.json();
    if (!requestData?.eventId || !requestData?.guestName || requestData.guestName.trim().length == 0) {
        return NextResponse.json('Fields eventId and guestName are required and cannot be empty.', { status: 400 });
    }
    
    if (requestData.guestName.trim().length < 3) {
        return NextResponse.json([]);
    }

    const sessionCookie = getSessionFromCookie(request);
    if (!sessionCookie)
        return NextResponse.json([], { status: 401 });

    if (process.env.APP_ENVIRONMENT == 'local') {
        return NextResponse.json(mockGuestListSearch);
    }

    const guestSearchRequest: Request = new Request(`${process.env.APP_API_URL}/api/events/${requestData.eventId}/guests?guestName=${requestData.guestName}`);
    addEventHeaders(guestSearchRequest, sessionCookie, requestData.eventId);    

    const guestSearchResponse = await fetch(guestSearchRequest);
    if (guestSearchResponse.status == 401) {
        return NextResponse.json([], { status: guestSearchResponse.status });
    }
    else if (!guestSearchResponse.ok) {
        return NextResponse.json([], { status: guestSearchResponse.status });
    }

    const guestSearchData: EventGuest[] = await guestSearchResponse.json();
    return NextResponse.json(guestSearchData);
}
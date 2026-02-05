import { addEventHeaders } from "@/app/lib/auth/api";
import { deleteSessionCookie, getSessionFromCookie } from "@/app/lib/auth/cookie";
import { NextRequest, NextResponse } from "next/server";

export interface GuestLoginRequestModel {
    guestId: number;
    eventId: string;
}

interface GuestLoginDto {
    GuestId: number;
}

export async function POST(request: NextRequest) {
    const guestData: GuestLoginRequestModel = await request.json();
    if (!guestData?.eventId || !guestData?.guestId)
        return NextResponse.json("One or more required fields are missing.", { status: 400 });

    const sessionCookie = getSessionFromCookie(request);
    if (!sessionCookie)
        return NextResponse.json('Invalid session.', { status: 401 });

    const guestLoginData: GuestLoginDto = {
        GuestId: guestData.guestId
    };

    const guestLoginRequest: Request = new Request(`${process.env.APP_API_URL}/auth/guest`,
        {
            method: 'POST',
            body: JSON.stringify(guestLoginData),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );
    addEventHeaders(guestLoginRequest, sessionCookie, guestData.eventId);

    const guestLoginResponse = await fetch(guestLoginRequest);

    if (!guestLoginResponse.ok) {
        const badResponse = NextResponse.json('Epic Bruh Moment.', { status: guestLoginResponse.status });
        deleteSessionCookie(badResponse);
        return badResponse;
    }

    return NextResponse.json('Start farting.')
}
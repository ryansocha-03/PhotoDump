import { addEventHeaders } from "@/app/lib/auth/api";
import { deleteSessionCookie, EVENT_HEADER_NAME, SESSION_COOKIE_NAME } from "@/app/lib/auth/cookie";
import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";

export async function GET(request: NextRequest) {
    const cookieStore = await cookies();
    const sessionId = cookieStore.get(SESSION_COOKIE_NAME)?.value;

    if (!sessionId)
        return NextResponse.json({}, { status: 401 });

    const eventId = request.headers.get(EVENT_HEADER_NAME);
    if (!eventId)
        return NextResponse.json({}, { status: 400 });

    const eventPhotoDownload = new Request(`${process.env.APP_API_URL}/media/download`);
    addEventHeaders(eventPhotoDownload, sessionId, eventId);

    const eventDownloadResponse = await fetch(eventPhotoDownload);
    
    if (eventDownloadResponse.status == 401) {
        const badResponse = NextResponse.json({}, { status: eventDownloadResponse.status });
        deleteSessionCookie(badResponse);
        return badResponse;
    }
    else if (!eventDownloadResponse.ok) {
        const badResponse = NextResponse.json({}, { status: eventDownloadResponse.status });
        return badResponse;
    }

    return NextResponse.json(await eventDownloadResponse.json());
}
import { addEventHeaders } from "@/app/lib/auth/api";
import { deleteSessionCookie, EVENT_HEADER_NAME, SESSION_COOKIE_NAME } from "@/app/lib/auth/cookie";
import { cookies, headers } from "next/headers";
import { NextRequest, NextResponse } from "next/server";

export interface FileUploadRequest {
    MediaUploadInfo: FileUploadInfo[];
    IsPrivate: boolean;
}

export interface FileUploadInfo {
    FileName: string;
    FileSize: number;
} 

export async function POST(request: NextRequest) {
    const cookieStore = await cookies();
    const sessionId = cookieStore.get(SESSION_COOKIE_NAME)?.value;

    if (!sessionId) return NextResponse.json({}, { status: 401 });

    const eventId = request.headers.get(EVENT_HEADER_NAME);
    if (!eventId) return NextResponse.json({}, { status: 400 });

    const mediaUploadRequest = new Request(`${process.env.APP_API_URL}/media/upload`,
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }
    );

    addEventHeaders(mediaUploadRequest, sessionId, eventId);

    const mediaUploadResponse = await fetch(mediaUploadRequest);
    
    if (mediaUploadResponse.status == 401) {
        const badResponse = NextResponse.json({}, { status: mediaUploadResponse.status });
        deleteSessionCookie(badResponse);
        return badResponse;
    }
    else if (!mediaUploadResponse.ok) {
        const badResponse = NextResponse.json({}, { status: mediaUploadResponse.status });
        return badResponse;
    }

   return mediaUploadResponse;
}
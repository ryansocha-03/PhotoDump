import { addEventHeaders } from "@/app/lib/auth/api";
import { deleteSessionCookie, EVENT_HEADER_NAME, SESSION_COOKIE_NAME } from "@/app/lib/auth/cookie";
import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";

export async function POST(
    request: NextRequest,
    { params }: { params: Promise<{ publicFileId: string }> }
) {
    const cookieStore = await cookies();
    const sessionId = cookieStore.get(SESSION_COOKIE_NAME)?.value;

    if (!sessionId)
        return NextResponse.json({}, { status: 401 });

    const eventId = request.headers.get(EVENT_HEADER_NAME);
    if (!eventId)
        return NextResponse.json({}, { status: 400 });

    const { publicFileId } = await params;
    const completionRequest = new Request(`${process.env.APP_API_URL}/media/upload/${encodeURIComponent(publicFileId)}/complete`, {
        method: "POST"
    });

    addEventHeaders(completionRequest, sessionId, eventId);

    const completionResponse = await fetch(completionRequest, { cache: "no-store" });

    if (completionResponse.status == 401) {
        const badResponse = NextResponse.json({}, { status: completionResponse.status });
        deleteSessionCookie(badResponse);
        return badResponse;
    }

    if (!completionResponse.ok)
        return NextResponse.json({}, { status: completionResponse.status });

    return new NextResponse(null, { status: completionResponse.status });
}

import { setSecureCookie } from "@/app/lib/auth/cookie";
import { redirect } from "next/navigation";
import { NextRequest, NextResponse } from "next/server";

export interface EventPasswordSubmission {
    eventPassword: string,
    eventPublicId: string
}

interface EventSessionData {
    sessionId: string,
    expiresAt: string
}

interface EventAuth {
    EventKey: string
}

export async function POST(request: NextRequest): Promise<NextResponse> {
    let requestData: EventPasswordSubmission;
    try {
        requestData = await request.json();
    }
    catch (e) {
        return NextResponse.json("Request no good. JSON please", { status: 400 })
    }

    if (requestData?.eventPassword?.trim().length == 0
        || requestData?.eventPublicId?.trim().length == 0) {
        return NextResponse.json("Fields eventPassword and eventPublicId are required and cannot be empty.", { status: 400 })
    }

    if (process.env.APP_ENVIRONMENT == 'local') {
        return NextResponse.json({});
    }

    const authBody: EventAuth = {
        EventKey: requestData.eventPassword
    }

    const eventAuthResponse = await fetch(
        `${process.env.APP_API_URL}/api/events/${requestData.eventPublicId}/auth`,
        {
            method: 'POST',
            body: JSON.stringify(authBody),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    )

    if (eventAuthResponse.status == 401)
       return NextResponse.json("Incorrect password.", { status: 401 })
    else if (!eventAuthResponse.ok)
        return NextResponse.json("Issue when validating event password.", { status: eventAuthResponse.status })

    const sessionData: EventSessionData = await eventAuthResponse.json();
    const response = NextResponse.json({});
    setSecureCookie(response, sessionData.sessionId, sessionData.expiresAt);

    return response;
}
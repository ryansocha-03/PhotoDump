import { NextRequest, NextResponse } from "next/server";

export const SESSION_COOKIE_NAME = 'sid';

export const SESSION_HEADER_NAME = 'X-Session-Id';

export const EVENT_HEADER_NAME = 'X-Event-Public-Id';

export function setSecureCookie(response: NextResponse, sessionId: string, expiresAt: string) {
    response.cookies.set(
        SESSION_COOKIE_NAME,
        sessionId,
        {
            httpOnly: true,
            sameSite: 'strict',
            secure: true,
            path: '/',
            expires: new Date(expiresAt)
        }
    )
}

export function getSessionFromCookie(request: NextRequest) {
    return request.cookies.get(SESSION_COOKIE_NAME)?.value?.trim();
}
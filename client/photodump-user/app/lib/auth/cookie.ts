import { NextRequest, NextResponse } from "next/server";

export const SESSION_COOKIE_NAME = 'sid';

export const SESSION_HEADER_NAME = 'X-Session-Id';

export const EVENT_HEADER_NAME = 'X-Event-Id';

export function setSecureCookie(response: NextResponse, sessionId: string, expiresAt: Date) {
    response.cookies.set(
        SESSION_COOKIE_NAME,
        sessionId,
        {
            httpOnly: true,
            sameSite: 'strict',
            secure: true,
            path: '/',
            expires: expiresAt
        }
    )
}

export function getSessionFromCookie(request: NextRequest) {
    return request.cookies.get(SESSION_COOKIE_NAME)?.value?.trim();
}

export function deleteSessionCookie(response: NextResponse) {
    response.cookies.delete(SESSION_COOKIE_NAME);
}
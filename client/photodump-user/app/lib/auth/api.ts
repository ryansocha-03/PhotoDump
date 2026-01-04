import { EVENT_HEADER_NAME, SESSION_COOKIE_NAME, SESSION_HEADER_NAME } from "./cookie";

export function addEventHeaders(request: Request, sessionId: string, eventId: string) {
    request.headers.append(SESSION_HEADER_NAME, sessionId);
    request.headers.append(EVENT_HEADER_NAME, eventId);
}
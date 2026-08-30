import { EVENT_HEADER_NAME } from "@/app/lib/auth/cookie";

/**
 * Wrapper function for making API requests from the client. It handles redirection for unauthorized (401) and forbidden (403) responses by redirecting the user to the event page.
 */
export default async function clientEventRequest(eventPublicId: string, endpoint: string, options?: RequestInit): Promise<Response> {
    const request = new Request(`/api${endpoint}`, options);
    request.headers.set(EVENT_HEADER_NAME, eventPublicId);

    const response = await fetch(request);

    if (response.status === 401) {
        window.location.href = `/e/${eventPublicId}`;
    }
    else if (response.status === 403) {
        window.location.href = `/e/${eventPublicId}`;
    }

    return response;
}
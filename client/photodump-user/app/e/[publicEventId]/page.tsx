import { addEventHeaders } from "@/app/lib/auth/api";
import { SESSION_COOKIE_NAME } from "@/app/lib/auth/cookie";
import { SessionTypeModel, SessionTypes } from "@/app/lib/auth/session-types";
import EventPassword from "@/app/ui/event-password";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";

export default async function Page({
    params,
}: {
    params: Promise<{publicEventId: string}>
}) {
    const { publicEventId } = await params;
    const cookieStore = await cookies();
    const sessionId = cookieStore.get(SESSION_COOKIE_NAME)?.value;

    if (sessionId) {
        const sessionValidationRequest = new Request(`${process.env.APP_API_URL}/auth/validate`);
        addEventHeaders(sessionValidationRequest, sessionId, publicEventId);
        const sessionValidationResponse = await fetch(sessionValidationRequest);
        if (sessionValidationResponse.ok) {
            const sessionTypeData: SessionTypeModel = await sessionValidationResponse.json();
            if (sessionTypeData.sessionType == SessionTypes.Anonymous)
                redirect(`/e/${publicEventId}/guests`)
            else if (sessionTypeData.sessionType == SessionTypes.Guest)
                redirect(`/e/${publicEventId}/photos`)
        }
    }

    return (
        <div className="flex flex-col items-center">
            <EventPassword eventId={publicEventId} />
        </div>
    )
}
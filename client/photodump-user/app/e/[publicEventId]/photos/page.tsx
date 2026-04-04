import { SESSION_COOKIE_NAME } from "@/app/lib/auth/cookie";
import { SessionTypeModel, SessionTypes } from "@/app/lib/auth/session-types";
import { getEventThumbnailUrls } from "@/app/lib/media/data";
import PhotoWrapper from "@/app/ui/photos/photo-wrapper";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { addEventHeaders } from "@/app/lib/auth/api";

export default async function EventPhotosPage({
    params
}: {
    params: Promise<{ publicEventId: string }>
}) {
    const { publicEventId } = await params;
    const cookieStore = await cookies();
    const sessionId = cookieStore.get(SESSION_COOKIE_NAME)?.value;

    if (sessionId) {
        const sessionValidationRequest = new Request(`${process.env.APP_API_URL}/auth/validate`)
        addEventHeaders(sessionValidationRequest, sessionId, publicEventId);
        const sessionValidationResponse = await fetch(sessionValidationRequest);
        if (sessionValidationResponse.ok) {
            const sessionTypeData: SessionTypeModel = await sessionValidationResponse.json();
            if (sessionTypeData.sessionType == SessionTypes.Anonymous)
                redirect(`/e/${publicEventId}/guests`)
        } else
            redirect(`/e/${publicEventId}`);
    } else {
        redirect(`/e/${publicEventId}`);
    }

    const thumbnailUrlData = await getEventThumbnailUrls(sessionId, publicEventId);
    const initialThumbnailPage = thumbnailUrlData.code == 200 && thumbnailUrlData.data
        ? thumbnailUrlData.data
        : {
            items: [],
            hasNext: false,
            nextCursor: null
        };

    return (
        <>
            <PhotoWrapper
                publicEventId={publicEventId}
                initialThumbnailPage={initialThumbnailPage}
            />
        </>
    )
}

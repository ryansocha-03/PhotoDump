import { SESSION_COOKIE_NAME } from "@/app/lib/auth/cookie";
import PhotoWrapper from "@/app/ui/photos/photo-wrapper";
import { cookies } from "next/headers";
import { redirect } from "next/navigation";

export default async function EventPhotosPage({
    params
}: {
    params: Promise<{ publicEventId: string }>
}) {
    const { publicEventId } = await params;
    const cookieStore = await cookies();
    const sessionId = cookieStore.get(SESSION_COOKIE_NAME)?.value;

    if (!sessionId) {
        redirect(`/e/${publicEventId}`);
    }

    return (
        <>
            <PhotoWrapper
                publicEventId={publicEventId}
            />
        </>
    )
}

import GuestListSearch from "@/app/ui/guest-list-search";

export default async function Page({
    params
}: {
    params: Promise<{publicEventId: string}>
}) {
    const { publicEventId } = await params;

    return (
        <div className="flex items-center justify-center">
            <GuestListSearch eventId={publicEventId}/>
        </div>
    )
}
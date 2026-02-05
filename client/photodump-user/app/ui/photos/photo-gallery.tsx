'use client'

import { EVENT_HEADER_NAME } from "@/app/lib/auth/cookie";
import { redirect, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import ThumbnailSkeleton from "./thumbnail-skeleton";
import Image from "next/image";

export default function PhotoGallery({
    eventId
}: {
    eventId: string,
}) {
    const [loadingThumbnails, setLoadingThumbnails] = useState<boolean>(true);
    const [thumbnails, setThumbnails] = useState<string[]>([]);
    const router = useRouter();

    useEffect(() => {
        const getThumbnails = async () => {
            const thumbnailRequest = new Request('/api/events/photos/download');
            thumbnailRequest.headers.append(EVENT_HEADER_NAME, eventId);

            const thumbnailResponse = await fetch(thumbnailRequest);

            if (thumbnailResponse.status == 401) {
                router.push(`/e/${eventId}`);
            } else if (!thumbnailResponse.ok) {
                throw Error("Something went wrong with downloading");
            } else {
                const thumbnailUrls: string[] = await thumbnailResponse.json();
                setThumbnails(thumbnailUrls);
                setLoadingThumbnails(false);
            }
        }

        getThumbnails();
    }, [])

    return (
        <div className="grid grid-cols-3 gap-6">
            {
                loadingThumbnails 
                ? <ThumbnailSkeleton />
                : thumbnails.length  == 0
                ? <div>No event photos yet. Upload public photos to share with other guests.</div>
                : thumbnails.map((t, i) => (
                    <img
                        key={`thumbnail-${i}`}
                        src={t}
                    />
                ))
            }
        </div>
    )
}
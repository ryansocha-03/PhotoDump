'use client'

import { PaginatedThumbnailUrls } from "@/app/lib/media/data"
import { FilledButton, OutlinedButton } from "../buttons";
import clientEventRequest from "@/app/lib/api-client";
import { useEffect, useState } from "react";
import Image from "next/image";

export default function PhotoWrapperV2({
    publicEventId
}: {
    publicEventId: string
}) {
    const [eventGalleryData, setEventGalleryData] = useState<PaginatedThumbnailUrls | null>(null);
    const [isLoadingGallery, setIsLoadingGallery] = useState<boolean>(true);

    const uploadMediaClickHandler = () => {
        console.log("Upload Media Clicked");
    }

    useEffect(() => {
        const fetchGalleryContent = async () => {
            try {
                const response = await clientEventRequest(publicEventId, "/events/photos/download/gallery");
                if (!response.ok) {
                    console.error("Failed to fetch gallery content:", response.statusText);
                    return;
                }
                const data: PaginatedThumbnailUrls = await response.json();
                setEventGalleryData(data);
                console.log("Gallery content fetched successfully:", data);
            } catch (error) {
                console.error("Error fetching gallery content:", error);
            }

            setIsLoadingGallery(false);
        };

        fetchGalleryContent();
    }, []);

    return (
        <>
            <div className="fixed bottom-6 right-6 z-50 bg-(--foreground) text-(--background) rounded-full p-2 hover:cursor-pointer" onClick={uploadMediaClickHandler}> 
                <svg xmlns="http://w3.org" fill="none" viewBox="0 0 24 24" strokeWidth="2.5" stroke="currentColor" className="w-8 h-8">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
                </svg>
            </div>
            {eventGalleryData && eventGalleryData.items.length > 0 ? (
                <div className="grid grid-cols-3 gap-2 mt-4">
                    {eventGalleryData.items.map((thumbnailUrl, index) => (
                        <div key={index} className="relative aspect-square">
                            <img src={thumbnailUrl} alt={`Thumbnail ${index + 1}`} className="aspect-square object-cover" />
                        </div>
                    ))}
                </div>
            ) : (
                <div className="flex justify-center items-center h-64 mt-4">
                    <p>No event images available. Upload some!</p>
                </div>
            )}
        </>
    )
}
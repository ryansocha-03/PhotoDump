'use client'

import PhotoItem from "./photo-item"

export default function PhotoGallery({
    thumbnailUrls
}: {
    thumbnailUrls: string[]
}) {

    return (
        <div className="grid grid-cols-3 gap-2 lg:gap-6">
            {
                thumbnailUrls.length == 0
                ? <div>No event photos yet. Upload public photos to share with other guests.</div>
                : thumbnailUrls.map((thumbnailUrl, i) => (
                    <PhotoItem key={`thumbnail-${i}`} thumbnailUrl={thumbnailUrl}/>
                ))
            }
        </div>
    )
}

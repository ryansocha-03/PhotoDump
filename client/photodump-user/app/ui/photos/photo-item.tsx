'use client'

import { useState } from "react"

export default function PhotoItem({
    thumbnailUrl
}: {
    thumbnailUrl: string 
}) {
    const [loadedThumbnailUrl, setLoadedThumbnailUrl] = useState<string | null>(null);
    const isLoaded = loadedThumbnailUrl == thumbnailUrl;

    const loadHandler = () => {
        setLoadedThumbnailUrl(thumbnailUrl)
    }

    return (
        <div className="flex items-center relative w-full overflow-hidden aspect-square rounded-md hover:cursor-pointer">
            {!isLoaded && 
                <div className="absolute inset-0 bg-gray-400 animate-pulse h-full" />
            }

            <img 
                src={thumbnailUrl}
                alt="thumbnail"
                onLoad={() => loadHandler()}
                onError={() => setLoadedThumbnailUrl(null)}
                className="w-full h-full object-cover"
            />
        </div>
    )
}

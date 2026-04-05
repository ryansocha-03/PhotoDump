'use client'

import { useCallback, useState } from "react"

export default function PhotoItem({
    thumbnailUrl,
    selectedIndex,
    openPhotoViewer,
    registerThumbnailImage
}: {
    thumbnailUrl: string,
    selectedIndex: number,
    openPhotoViewer: (selectedIndex: number) => void,
    registerThumbnailImage: (selectedIndex: number, image: HTMLImageElement | null) => void
}) {
    const [loadedThumbnailUrl, setLoadedThumbnailUrl] = useState<string | null>(null);
    const isLoaded = loadedThumbnailUrl == thumbnailUrl;

    const imageRef = useCallback((node: HTMLImageElement | null) => {
        registerThumbnailImage(selectedIndex, node);

        if (!node) {
            return;
        }

        if (node.complete && node.naturalWidth > 0) {
            setLoadedThumbnailUrl(thumbnailUrl);
            return;
        }

        setLoadedThumbnailUrl(null);
    }, [registerThumbnailImage, selectedIndex, thumbnailUrl]);

    const loadHandler = () => {
        setLoadedThumbnailUrl(thumbnailUrl)
    }

    return (
        <button
            type="button"
            onClick={() => openPhotoViewer(selectedIndex)}
            className="relative flex aspect-square w-full items-center overflow-hidden hover:cursor-pointer"
        >
            {!isLoaded && 
                <div className="absolute inset-0 bg-gray-400 animate-pulse h-full" />
            }

            <img 
                ref={imageRef}
                src={thumbnailUrl}
                alt="thumbnail"
                onLoad={() => loadHandler()}
                onError={() => setLoadedThumbnailUrl(null)}
                className="w-full h-full object-cover"
            />
        </button>
    )
}

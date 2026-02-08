'use client'

import { useEffect, useState } from "react"

export default function PhotoItem({
    photoData
}: {
    photoData: string 
}) {
    const [isLoaded, setIsLoaded]= useState(false);

    useEffect(() => {
        const image = new Image();
        image.src = photoData;
        if (image.complete) setIsLoaded(true);
    }, [photoData]);

    const loadHandler = () => {
        setIsLoaded(true)
    }

    return (
        <div className="flex items-center relative w-full overflow-hidden aspect-square rounded-md hover:cursor-pointer">
            {!isLoaded && 
                <div className="absolute inset-0 bg-gray-400 animate-pulse h-full" />
            }

            <img 
                src={photoData}
                alt="thumbnail"
                onLoad={() => loadHandler()}
                className="w-full h-full object-cover"
            />
        </div>
    )
}
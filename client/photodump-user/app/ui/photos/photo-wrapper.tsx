'use client'

import { useState } from "react"
import { FilledButton, OutlinedButton } from "../buttons"
import PhotoGallery from "./photo-gallery"

export enum PhotoMode {
    Default,
    Select,
    Add
}

export default function PhotoWrapper({
    eventId
}: {
    eventId: string
}) {
    const [photoMode, setPhotoMode] = useState<PhotoMode>(PhotoMode.Default);

    const switchToSelectMode = () => {
        setPhotoMode(PhotoMode.Select);
    }

    const switchToAddMode = () => {
        setPhotoMode(PhotoMode.Add)
    }

    return (
        <>
            <div className="flex justify-between mb-5">
                <p className="text-lg">Event Photos</p>
                <div className="flex gap-2 h-[35px]">
                    <div className="w-[75px]">
                        <OutlinedButton text="Select" clickHandler={switchToSelectMode} />
                    </div>
                    <div className="w-[75px]">
                        <FilledButton text="+ Add" clickHandler={switchToAddMode} />
                    </div>
                </div>
            </div>
            <PhotoGallery eventId={eventId} /> 
        </>
    )
}
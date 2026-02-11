'use client'

import { useState } from "react"
import { FilledButton, OutlinedButton } from "../buttons"
import PhotoGallery from "./photo-gallery"
import AddMediaModal from "./photo-add-modal"
import { Dialog, DialogBackdrop } from "@headlessui/react"

export enum PhotoMode {
    Default,
    Select,
    Add
}

export default function PhotoWrapper({
    mediaMetadata
}: {
    mediaMetadata: string[]
}) {
    const [photoMode, setPhotoMode] = useState<PhotoMode>(PhotoMode.Default);

    return (
        <>
           <AddMediaModal mode={photoMode} closeHandler={(m: PhotoMode) => setPhotoMode(PhotoMode.Default)} /> 

            <div className="flex justify-between mb-5">
                <p className="text-lg">Event Photos</p>
                <div className="flex gap-2 h-[35px]">
                    {photoMode == PhotoMode.Default &&
                        <div className="w-[75px]">
                            <OutlinedButton text="Select" clickHandler={() => setPhotoMode(PhotoMode.Select)} />
                        </div>
                    }
                    <div className="w-[75px]">
                        {photoMode == PhotoMode.Default 
                            ? <FilledButton text="+ Add" clickHandler={() => setPhotoMode(PhotoMode.Add)} />
                            : <FilledButton text="Cancel" clickHandler={() => setPhotoMode(PhotoMode.Default)} />
                        }
                    </div>
                </div>
            </div>
            <PhotoGallery mediaMetadata={mediaMetadata} /> 
        </>
    )
}
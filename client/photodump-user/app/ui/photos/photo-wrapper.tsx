'use client'

import { useState } from "react";
import { FileUploadRequest, MediaUploadTicket } from "@/app/lib/media/data";
import clientEventRequest from "@/app/lib/api-client";
import toast from "react-hot-toast";

export default function PhotoWrapper({
    publicEventId
}: {
    publicEventId: string
}) {
    const [isUploading, setIsUploading] = useState(false);

    const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const files = event.target.files;
        if (!files || files.length === 0) return;

        setIsUploading(true);
        toast.success("Uploading photos. This may take a few minutes depending on your connection speed.");
        
        const filesArray = Array.from(files);   
        var uploadsAreSuccess = true;

        try {
            const fileMetaData: FileUploadRequest = {
                MediaUploadInfo: filesArray.map(file => ({
                    FileName: file.name,
                    FileExtension: file.name.split('.').pop() || '',    
                    FileSize: file.size
                }))
            };

            const uploadResponse = await clientEventRequest(publicEventId, "/events/photos/upload", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(fileMetaData)
            });

            if (uploadResponse.status == 401 || uploadResponse.status == 403) {
                window.location.href = `/e/${publicEventId}`;
            }

            if (!uploadResponse.ok) {
                console.error("Failed to get upload tickets:", uploadResponse.status);
                return;
            }

            const uploadTickets : MediaUploadTicket[] = await uploadResponse.json();

            const uploadPromises = filesArray.map((file, index) => {
                return fetch(uploadTickets[index].fileUploadUrl, {
                    method: "PUT",
                    body: file
                }).then(response => {
                    if (!response.ok) {
                        uploadsAreSuccess = false;
                    }
                });
            });

            await Promise.all(uploadPromises);

        }
        catch (error) {
            console.error("Error uploading files:", error); 
        }
        finally {
            setIsUploading(false);
            if (uploadsAreSuccess) {
                toast.success("Photos uploaded successfully!");
            }
            else {
                toast.error("Some photos failed to upload. Please try again.");
            }
        }
    }

    return (
        <>
            <div className="text-center text-xl my-4">
                Tap the button below to share your photos with us
            </div>

            <div className="w-100vw flex justify-center">
                <label
                    htmlFor="file-input"
                    className={`bg-(--foreground) text-(--background) py-2 px-4 rounded-full w-50 text-center ${isUploading ? "opacity-50 cursor-not-allowed" : "cursor-pointer"}`}
                >
                   {isUploading ? "Uploading..." : "Select Photos"} 
                </label>
                <input
                    disabled={isUploading}
                    type="file"
                    accept="image/*"
                    id="file-input"
                    multiple
                    className="hidden w-full"
                    onChange={handleFileUpload}
                />
            </div>
       </>
    )
}
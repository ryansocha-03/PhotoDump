import { Dialog, DialogBackdrop, DialogPanel, DialogTitle, Field, Label, Switch } from "@headlessui/react"
import { PhotoMode } from "./photo-wrapper"
import { useState } from "react";
import { FilledButton, OutlinedButton } from "../buttons";
import { FileUploadInfo, FileUploadRequest } from "@/app/api/events/photos/upload/route";

export default function AddMediaModal({
    mode,
    closeHandler
}: {
    mode: PhotoMode,
    closeHandler: (mode: PhotoMode) => void
}) {
    const [userFiles, setUserFiles] = useState<FileUploadInfo[]>([]);
    const [isPublic, setIsPublic] = useState(true);

    const setSelectedFiles = (files: FileList | null) => {
        const toUpload: FileUploadInfo[] = []
        if (files) {
            for (let i = 0;i < files.length;i++) {
                toUpload.push({
                    FileName: files[i].name,
                    FileSize: files[i].size
                })
            }
        }
        setUserFiles(toUpload);
    }

    const handleSubmitFiles = () => {
        const uploadData: FileUploadRequest = {
            MediaUploadInfo:  userFiles,
            IsPrivate: !isPublic
        };


        // TODO: Do rest of submit behabior (call API, upload via URLs, show upload status pop up)
    }

    return (
        <Dialog open={mode == PhotoMode.Add} onClose={() => closeHandler(PhotoMode.Default)} className="relative z-50">
            <DialogBackdrop className="fixed inset-0 bg-black/60" />
            <div className="fixed inset-0 flex w-screen items-center justify-center">
                <DialogPanel className={'bg-(--background) rounded-sm p-4 border border-(--foreground) p-4'}>
                    <DialogTitle className={'text-xl font-bold mb-3'}>Import photo/video</DialogTitle>
                    <label className="border border-white py-1 px-4">
                        Browse
                        <input 
                            onChange={(e) => setSelectedFiles(e.target.files)}
                            className="hidden"
                            type="file"
                            accept=".jpeg, .png, .jpg"
                            multiple
                        />
                    </label>
                    <span className="ml-3">{`${userFiles.length} files selected.`}</span>
                    <Field className={'my-2'}>
                        <p>Share photos with guests</p>
                        <Switch
                            checked={isPublic}
                            onChange={setIsPublic}
                            className={"group inline-flex h-6 w-11 items-center rounded-full bg-gray-200 transition data-checked:bg-blue-600"}
                        >
                            <span className="size-4 translate-x-1 rounded-full bg-white transition group-data-checked:translate-x-6" />
                        </Switch>
                        <p className="text-wrap">{isPublic ? "Uploaded photos will visible to all event guests." : "Uploaded photos will be visible to event admins only."}</p>
                    </Field>
                    <div className="flex gap-2 justify-center">
                        <div className="grow max-w-100">
                            <OutlinedButton text="Cancel" clickHandler={() => closeHandler(PhotoMode.Default)}/>
                        </div>
                        <div className="grow max-w-100">
                            <FilledButton text="Upload" clickHandler={() => console.log("bruh")} />
                        </div>
                    </div>
              </DialogPanel>
            </div>
        </Dialog>
    )
}
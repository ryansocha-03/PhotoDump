'use client'

import {
    Dialog,
    DialogBackdrop,
    DialogPanel,
    DialogTitle,
    Field,
    Switch
} from "@headlessui/react"
import {
    acknowledgeCompletedUpload,
    FileUploadInfo,
    FileUploadRequest,
    requestMediaUploadTickets,
    uploadFileToContentStore
} from "@/app/lib/media/data";
import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { PhotoMode } from "./photo-wrapper"
import { FilledButton, OutlinedButton } from "../buttons";

interface UploadSummary {
    kind: "success" | "partial" | "error",
    message: string,
    failedFileNames: string[]
}

export default function AddMediaModal({
    mode,
    publicEventId,
    closeHandler
}: {
    mode: PhotoMode,
    publicEventId: string,
    closeHandler: () => void
}) {
    const [userFiles, setUserFiles] = useState<File[]>([]);
    const [isPublic, setIsPublic] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [uploadSummary, setUploadSummary] = useState<UploadSummary | null>(null);
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const router = useRouter();

    const clearSelectedFiles = () => {
        setUserFiles([]);

        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }
    };

    const resetModalState = () => {
        clearSelectedFiles();
        setIsPublic(true);
        setUploadSummary(null);
    };

    const handleClose = () => {
        if (isSubmitting) {
            return;
        }

        resetModalState();
        closeHandler();
    };

    const setSelectedFiles = (files: FileList | null) => {
        if (!files) {
            clearSelectedFiles();
            setUploadSummary(null);
            return;
        }

        setUploadSummary(null);
        setUserFiles(Array.from(files));
    };

    const buildUploadRequest = (files: File[], isPrivate: boolean): FileUploadRequest => {
        const mediaUploadInfo: FileUploadInfo[] = files.map((file) => ({
            FileName: file.name,
            FileSize: file.size
        }));

        return {
            MediaUploadInfo: mediaUploadInfo,
            IsPrivate: isPrivate
        };
    };

    const handleSubmitFiles = async () => {
        if (isSubmitting || userFiles.length == 0) {
            return;
        }

        setIsSubmitting(true);
        setUploadSummary(null);

        try {
            const uploadData = buildUploadRequest(userFiles, !isPublic);
            const uploadTicketResponse = await requestMediaUploadTickets(publicEventId, uploadData);

            if (uploadTicketResponse.code == 401) {
                router.push(`/e/${publicEventId}`);
                return;
            }

            const uploadTickets = uploadTicketResponse.data;
            if (uploadTicketResponse.code != 200 || !uploadTickets) {
                setUploadSummary({
                    kind: "error",
                    message: "Unable to start uploads right now. Please try again.",
                    failedFileNames: []
                });
                return;
            }

            if (uploadTickets.length != userFiles.length || uploadTickets.some((ticket) => !ticket.publicFileId || !ticket.fileUploadUrl)) {
                setUploadSummary({
                    kind: "error",
                    message: "Unable to start uploads right now. Please try again.",
                    failedFileNames: []
                });
                return;
            }

            let successCount = 0;
            const failedFileNames: string[] = [];

            for (let i = 0; i < userFiles.length; i++) {
                const currentFile = userFiles[i];
                const currentTicket = uploadTickets[i];
                if (!currentFile || !currentTicket) {
                    continue;
                }

                const uploadSucceeded = await uploadFileToContentStore(currentFile, currentTicket.fileUploadUrl);
                if (!uploadSucceeded) {
                    failedFileNames.push(currentFile.name);
                    continue;
                }

                const completionResponse = await acknowledgeCompletedUpload(publicEventId, currentTicket.publicFileId);
                if (completionResponse.code == 401) {
                    router.push(`/e/${publicEventId}`);
                    return;
                }

                if (completionResponse.code != 200 && completionResponse.code != 204) {
                    failedFileNames.push(currentFile.name);
                    continue;
                }

                successCount++;
            }

            clearSelectedFiles();

            if (successCount == userFiles.length) {
                setUploadSummary({
                    kind: "success",
                    message: `Submitted ${successCount} upload${successCount == 1 ? "" : "s"}. Photos will appear after processing completes.`,
                    failedFileNames: []
                });
                return;
            }

            if (successCount > 0) {
                setUploadSummary({
                    kind: "partial",
                    message: `Submitted ${successCount} of ${userFiles.length} uploads. Some photos could not be submitted, and successful uploads will appear after processing completes.`,
                    failedFileNames
                });
                return;
            }

            setUploadSummary({
                kind: "error",
                message: "No uploads were submitted. Please try again.",
                failedFileNames
            });
        } finally {
            setIsSubmitting(false);
        }
    };

    const summaryClassName = uploadSummary?.kind == "success"
        ? "border-green-600/50 bg-green-500/10 text-green-200"
        : uploadSummary?.kind == "partial"
            ? "border-amber-500/50 bg-amber-500/10 text-amber-100"
            : "border-red-500/50 bg-red-500/10 text-red-200";

    return (
        <Dialog open={mode == PhotoMode.Add} onClose={handleClose} className="relative z-50">
            <DialogBackdrop className="fixed inset-0 bg-black/60" />
            <div className="fixed inset-0 flex w-screen items-center justify-center p-4">
                <DialogPanel className="w-full max-w-lg rounded-sm border border-(--foreground) bg-(--background) p-4">
                    <DialogTitle className="mb-3 text-xl font-bold">Import photo/video</DialogTitle>
                    <div className="flex items-center gap-3">
                        <label className={`border border-white px-4 py-1 ${isSubmitting ? "cursor-not-allowed opacity-45" : "hover:cursor-pointer"}`}>
                            Browse
                            <input
                                ref={fileInputRef}
                                onChange={(e) => setSelectedFiles(e.target.files)}
                                className="hidden"
                                type="file"
                                accept=".jpeg, .png, .jpg"
                                multiple
                                disabled={isSubmitting}
                            />
                        </label>
                        <span>{`${userFiles.length} file${userFiles.length == 1 ? "" : "s"} selected.`}</span>
                    </div>

                    <Field className="my-3">
                        <p>Share photos with guests</p>
                        <Switch
                            checked={isPublic}
                            onChange={setIsPublic}
                            disabled={isSubmitting}
                            className="group inline-flex h-6 w-11 items-center rounded-full bg-gray-200 transition data-checked:bg-blue-600 disabled:cursor-not-allowed disabled:opacity-45"
                        >
                            <span className="size-4 translate-x-1 rounded-full bg-white transition group-data-checked:translate-x-6" />
                        </Switch>
                        <p className="text-wrap">
                            {isPublic
                                ? "Uploaded photos will be visible to all event guests."
                                : "Uploaded photos will be visible to event admins only."}
                        </p>
                    </Field>

                    {uploadSummary &&
                        <div className={`mb-3 border p-3 text-sm ${summaryClassName}`}>
                            <p>{uploadSummary.message}</p>
                            {uploadSummary.failedFileNames.length > 0 &&
                                <p className="mt-2 break-words">
                                    Failed: {uploadSummary.failedFileNames.join(", ")}
                                </p>
                            }
                        </div>
                    }

                    <div className="flex justify-center gap-2">
                        <div className="grow max-w-100">
                            <OutlinedButton
                                text="Cancel"
                                clickHandler={handleClose}
                                disabled={isSubmitting}
                            />
                        </div>
                        <div className="grow max-w-100">
                            <FilledButton
                                text={isSubmitting ? "Uploading..." : "Upload"}
                                clickHandler={() => void handleSubmitFiles()}
                                disabled={isSubmitting || userFiles.length == 0}
                            />
                        </div>
                    </div>
                </DialogPanel>
            </div>
        </Dialog>
    )
}

import { addEventHeaders } from "../auth/api";
import { EVENT_HEADER_NAME, SESSION_HEADER_NAME } from "../auth/cookie";
import { ApiResponseModel } from "../types";

export interface PaginatedThumbnailUrls {
    items: string[],
    hasNext: boolean,
    nextCursor: string | null
}

export interface FileUploadInfo {
    FileName: string;
    FileExtension: string;
    FileSize: number;
}

export interface FileUploadRequest {
    MediaUploadInfo: FileUploadInfo[],
    Privacy?: number 
}

export interface MediaUploadTicket {
    publicFileId: string,
    fileUploadUrl: string
}

/**
 * Gets the thumbnails for a gallery with pagination support.
 * @param sessionId The user's session ID for authentication.
 * @param publicEventId The public ID of the event for which to retrieve thumbnails.
 * @param cursor Optional cursor for pagination. If provided, the API will return the next page of results starting from this cursor.
 * @returns A promise that resolves to an ApiResponseModel containing PaginatedThumbnailUrls, which includes the list of thumbnail URLs, a flag indicating if there are more pages, and the next cursor for pagination.
 */
export async function getGalleryContent(sessionId: string, publicEventId: string, cursor?: string): Promise<ApiResponseModel<PaginatedThumbnailUrls>> {
    const thumbnailData: ApiResponseModel<PaginatedThumbnailUrls> = {
        code: 200,
        data: null
    };

    let thumbnailResponse: Response;
    try {
        const thumbnailUrl = new URL(`${process.env.APP_API_URL}/api/v1/media/download`);
        if (cursor) {
            thumbnailUrl.searchParams.set("cursor", cursor);
        }

        const thumbnailRequest = new Request(thumbnailUrl);
        addEventHeaders(thumbnailRequest, sessionId, publicEventId);
        thumbnailResponse = await fetch(thumbnailRequest, { cache: "no-store" });
    }
    catch {
        thumbnailData.code = 500;
        return thumbnailData;
    }

    thumbnailData.code = thumbnailResponse.status;
    if (!thumbnailResponse.ok) {
        return thumbnailData;
    }

    const thumbnailResponseJson: PaginatedThumbnailUrls = await thumbnailResponse.json();
    thumbnailData.data = {
        items: thumbnailResponseJson.items ?? [],
        hasNext: thumbnailResponseJson.hasNext ?? false,
        nextCursor: thumbnailResponseJson.nextCursor ?? null
    };

    return thumbnailData;
}

export async function getNextEventThumbnailUrls(publicEventId: string, cursor: string): Promise<ApiResponseModel<PaginatedThumbnailUrls>> {
    const thumbnailData: ApiResponseModel<PaginatedThumbnailUrls> = {
        code: 200,
        data: null
    };

    let thumbnailResponse: Response;
    try {
        thumbnailResponse = await fetch(`/api/events/photos/download?cursor=${encodeURIComponent(cursor)}`,
            {
                cache: "no-store",
                headers: {
                    [EVENT_HEADER_NAME]: publicEventId
                }
            }
        );
    }
    catch {
        thumbnailData.code = 500;
        return thumbnailData;
    }

    thumbnailData.code = thumbnailResponse.status;
    if (!thumbnailResponse.ok) {
        return thumbnailData;
    }

    const thumbnailResponseJson: PaginatedThumbnailUrls = await thumbnailResponse.json();
    thumbnailData.data = {
        items: thumbnailResponseJson.items ?? [],
        hasNext: thumbnailResponseJson.hasNext ?? false,
        nextCursor: thumbnailResponseJson.nextCursor ?? null
    };

    return thumbnailData;
}

export async function requestMediaUploadTickets(publicEventId: string, uploadRequest: FileUploadRequest): Promise<ApiResponseModel<MediaUploadTicket[]>> {
    const uploadData: ApiResponseModel<MediaUploadTicket[]> = {
        code: 200,
        data: null
    };

    let uploadResponse: Response;
    try {
        uploadResponse = await fetch("/api/events/photos/upload", {
            method: "POST",
            cache: "no-store",
            headers: {
                "Content-Type": "application/json",
                [EVENT_HEADER_NAME]: publicEventId
            },
            body: JSON.stringify(uploadRequest)
        });
    }
    catch {
        uploadData.code = 500;
        return uploadData;
    }

    uploadData.code = uploadResponse.status;
    if (!uploadResponse.ok) {
        return uploadData;
    }

    const uploadResponseJson = await uploadResponse.json();
    const uploadTickets = Array.isArray(uploadResponseJson) ? uploadResponseJson : [];
    uploadData.data = uploadTickets.map((ticket) => ({
        publicFileId: ticket?.publicFileId ?? "",
        fileUploadUrl: ticket?.fileUploadUrl ?? ""
    }));

    return uploadData;
}

export async function uploadFileToContentStore(file: File, uploadUrl: string): Promise<boolean> {
    try {
        const uploadResponse = await fetch(uploadUrl, {
            method: "PUT",
            body: file
        });

        return uploadResponse.ok;
    }
    catch {
        return false;
    }
}

export async function acknowledgeCompletedUpload(publicEventId: string, publicFileId: string): Promise<ApiResponseModel<null>> {
    const completionData: ApiResponseModel<null> = {
        code: 200,
        data: null
    };

    let completionResponse: Response;
    try {
        completionResponse = await fetch(`/api/events/photos/upload/${encodeURIComponent(publicFileId)}/complete`, {
            method: "POST",
            cache: "no-store",
            headers: {
                [EVENT_HEADER_NAME]: publicEventId
            }
        });
    }
    catch {
        completionData.code = 500;
        return completionData;
    }

    completionData.code = completionResponse.status;
    return completionData;
}

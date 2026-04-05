import { EVENT_HEADER_NAME } from "../auth/cookie";
import { ApiResponseModel } from "../types";

const SESSION_HEADER_NAME = "X-Session-Id";

export interface PaginatedThumbnailUrls {
    items: string[],
    hasNext: boolean,
    nextCursor: string | null
}

export interface FileUploadInfo {
    FileName: string,
    FileSize: number
}

export interface FileUploadRequest {
    MediaUploadInfo: FileUploadInfo[],
    IsPrivate: boolean
}

export interface MediaUploadTicket {
    publicFileId: string,
    fileUploadUrl: string
}

export async function getEventThumbnailUrls(sessionId: string, publicEventId: string, cursor?: string): Promise<ApiResponseModel<PaginatedThumbnailUrls>> {
    const thumbnailData: ApiResponseModel<PaginatedThumbnailUrls> = {
        code: 200,
        data: null
    };

    let thumbnailResponse: Response;
    try {
        const thumbnailUrl = new URL(`${process.env.APP_API_URL}/media/download`);
        if (cursor) {
            thumbnailUrl.searchParams.set("cursor", cursor);
        }

        const thumbnailRequest = new Request(thumbnailUrl);
        thumbnailRequest.headers.append(SESSION_HEADER_NAME, sessionId);
        thumbnailRequest.headers.append(EVENT_HEADER_NAME, publicEventId);
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

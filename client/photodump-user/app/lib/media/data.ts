import { addEventHeaders } from "../auth/api";
import { ApiResponseModel } from "../types";

export interface PaginatedThumbnailUrls {
    items: string[],
    hasNext: boolean,
    nextCursor: string | null
}

export async function getEventThumbnailUrls(sessionId: string, publicEventId: string): Promise<ApiResponseModel<PaginatedThumbnailUrls>> {
    const thumbnailData: ApiResponseModel<PaginatedThumbnailUrls> = {
        code: 200,
        data: null
    };

    let thumbnailResponse: Response;
    try {
        const thumbnailRequest = new Request(`${process.env.APP_API_URL}/media/download`);
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

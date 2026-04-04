import { ApiResponseModel } from "../types";

const SESSION_HEADER_NAME = "X-Session-Id";
const EVENT_HEADER_NAME = "X-Event-Public-Id";

export interface PaginatedThumbnailUrls {
    items: string[],
    hasNext: boolean,
    nextCursor: string | null
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

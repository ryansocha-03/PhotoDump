'use client'

import { PaginatedThumbnailUrls, getNextEventThumbnailUrls } from "@/app/lib/media/data"
import { useEffect, useRef, useState } from "react"
import { FilledButton, OutlinedButton } from "../buttons"
import PhotoGallery from "./photo-gallery"
import AddMediaModal from "./photo-add-modal"
import ThumbnailSkeleton from "./thumbnail-skeleton"
import { useRouter } from "next/navigation"

export enum PhotoMode {
    Default,
    Select,
    Add
}

export default function PhotoWrapper({
    publicEventId,
    initialThumbnailPage
}: {
    publicEventId: string,
    initialThumbnailPage: PaginatedThumbnailUrls
}) {
    const [photoMode, setPhotoMode] = useState<PhotoMode>(PhotoMode.Default);
    const [thumbnailUrls, setThumbnailUrls] = useState<string[]>(initialThumbnailPage.items);
    const [hasNext, setHasNext] = useState(initialThumbnailPage.hasNext);
    const [nextCursor, setNextCursor] = useState<string | null>(initialThumbnailPage.nextCursor);
    const [isLoadingMore, setIsLoadingMore] = useState(false);
    const scrollContainerRef = useRef<HTMLDivElement | null>(null);
    const loadMoreRef = useRef<HTMLDivElement | null>(null);
    const loadingMoreRef = useRef(false);
    const router = useRouter();

    useEffect(() => {
        if (!hasNext || !nextCursor || isLoadingMore || !loadMoreRef.current || !scrollContainerRef.current) {
            return;
        }

        const loadMoreNode = loadMoreRef.current;
        const scrollContainerNode = scrollContainerRef.current;
        const observer = new IntersectionObserver(
            (entries) => {
                if (!entries[0]?.isIntersecting || loadingMoreRef.current) {
                    return;
                }

                loadingMoreRef.current = true;
                setIsLoadingMore(true);

                void getNextEventThumbnailUrls(publicEventId, nextCursor)
                    .then((thumbnailPageResponse) => {
                        if (thumbnailPageResponse.code == 401) {
                            router.push(`/e/${publicEventId}`);
                            return;
                        }

                        if (thumbnailPageResponse.code != 200 || !thumbnailPageResponse.data) {
                            setHasNext(false);
                            return;
                        }

                        const nextPage = thumbnailPageResponse.data;
                        setThumbnailUrls((currentUrls) => [...currentUrls, ...nextPage.items]);
                        setHasNext(nextPage.hasNext);
                        setNextCursor(nextPage.nextCursor);
                    })
                    .finally(() => {
                        loadingMoreRef.current = false;
                        setIsLoadingMore(false);
                    });
            },
            {
                root: scrollContainerNode,
                rootMargin: "300px 0px"
            }
        );

        observer.observe(loadMoreNode);

        return () => observer.disconnect();
    }, [hasNext, isLoadingMore, nextCursor, publicEventId, router]);

    return (
        <>
           <AddMediaModal mode={photoMode} closeHandler={() => setPhotoMode(PhotoMode.Default)} /> 

            <div className="flex h-[calc(100dvh-12rem)] min-h-[24rem] flex-col overflow-hidden">
                <div className="mb-5 flex shrink-0 justify-between">
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

                <div
                    ref={scrollContainerRef}
                    className="min-h-0 flex-1 overflow-y-auto pr-1"
                >
                    <PhotoGallery thumbnailUrls={thumbnailUrls} /> 
                    {isLoadingMore &&
                        <div className="mt-2 grid grid-cols-3 gap-2 lg:mt-6 lg:gap-6">
                            <ThumbnailSkeleton />
                        </div>
                    }
                    {hasNext && <div ref={loadMoreRef} className="h-1 w-full" aria-hidden="true" />}
                </div>
            </div>
        </>
    )
}

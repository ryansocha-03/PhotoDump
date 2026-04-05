'use client'

import { PaginatedThumbnailUrls, getNextEventThumbnailUrls } from "@/app/lib/media/data"
import { useCallback, useEffect, useRef, useState } from "react"
import { FilledButton, OutlinedButton } from "../buttons"
import PhotoGallery from "./photo-gallery"
import AddMediaModal from "./photo-add-modal"
import ThumbnailSkeleton from "./thumbnail-skeleton"
import { useRouter } from "next/navigation"
import PhotoViewer, { PhotoViewerPhase, ViewerRect } from "./photo-viewer"

export enum PhotoMode {
    Default,
    Select,
    Add
}

const VIEWER_TRANSITION_MS = 220;
const THUMBNAIL_BORDER_RADIUS_PX = 0;
const VIEWER_BORDER_RADIUS_PX = 0;
const MOBILE_VIEWER_PADDING_PX = 16;
const DESKTOP_VIEWER_PADDING_PX = 32;
const MOBILE_VIEWER_TOP_PADDING_PX = 80;
const DESKTOP_VIEWER_TOP_PADDING_PX = 112;
const MOBILE_NAVIGATION_WIDTH_PX = 56;
const DESKTOP_NAVIGATION_WIDTH_PX = 64;
const MOBILE_NAVIGATION_GAP_PX = 8;
const DESKTOP_NAVIGATION_GAP_PX = 16;

function getViewerTargetRect(sourceImage: HTMLImageElement | null): ViewerRect {
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    const isMobile = viewportWidth < 768;
    const horizontalPadding = isMobile ? MOBILE_VIEWER_PADDING_PX : DESKTOP_VIEWER_PADDING_PX;
    const topPadding = isMobile ? MOBILE_VIEWER_TOP_PADDING_PX : DESKTOP_VIEWER_TOP_PADDING_PX;
    const bottomPadding = horizontalPadding;
    const navigationWidth = isMobile ? MOBILE_NAVIGATION_WIDTH_PX : DESKTOP_NAVIGATION_WIDTH_PX;
    const navigationGap = isMobile ? MOBILE_NAVIGATION_GAP_PX : DESKTOP_NAVIGATION_GAP_PX;
    const availableWidth = viewportWidth - (horizontalPadding * 2) - (navigationWidth * 2) - (navigationGap * 2);
    const availableHeight = viewportHeight - topPadding - bottomPadding;
    const maxWidth = Math.max(1, availableWidth);
    const maxHeight = Math.max(1, availableHeight);
    const sourceWidth = sourceImage?.naturalWidth || sourceImage?.width || maxWidth;
    const sourceHeight = sourceImage?.naturalHeight || sourceImage?.height || maxHeight;
    const scale = Math.min(maxWidth / sourceWidth, maxHeight / sourceHeight);
    const width = Math.max(1, sourceWidth * scale);
    const height = Math.max(1, sourceHeight * scale);
    const imageAreaLeft = horizontalPadding + navigationWidth + navigationGap;

    return {
        top: topPadding + ((availableHeight - height) / 2),
        left: imageAreaLeft + ((availableWidth - width) / 2),
        width,
        height
    };
}

function isRectVisibleInViewport(rect: DOMRect) {
    return rect.bottom > 0 && rect.right > 0 && rect.left < window.innerWidth && rect.top < window.innerHeight;
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
    const [selectedIndex, setSelectedIndex] = useState<number | null>(null);
    const [viewerPhase, setViewerPhase] = useState<PhotoViewerPhase>('closed');
    const [transitionImageUrl, setTransitionImageUrl] = useState<string | null>(null);
    const [transitionRect, setTransitionRect] = useState<ViewerRect | null>(null);
    const [transitionBorderRadius, setTransitionBorderRadius] = useState(THUMBNAIL_BORDER_RADIUS_PX);
    const [backdropOpacity, setBackdropOpacity] = useState(0);
    const scrollContainerRef = useRef<HTMLDivElement | null>(null);
    const loadMoreRef = useRef<HTMLDivElement | null>(null);
    const loadingMoreRef = useRef(false);
    const thumbnailImageRefs = useRef(new Map<number, HTMLImageElement>());
    const animationFrameRef = useRef<number | null>(null);
    const animationTimeoutRef = useRef<number | null>(null);
    const router = useRouter();

    useEffect(() => {
        return () => {
            if (animationFrameRef.current != null) {
                window.cancelAnimationFrame(animationFrameRef.current);
            }

            if (animationTimeoutRef.current != null) {
                window.clearTimeout(animationTimeoutRef.current);
            }
        };
    }, []);

    const clearAnimationState = useCallback(() => {
        if (animationFrameRef.current != null) {
            window.cancelAnimationFrame(animationFrameRef.current);
            animationFrameRef.current = null;
        }

        if (animationTimeoutRef.current != null) {
            window.clearTimeout(animationTimeoutRef.current);
            animationTimeoutRef.current = null;
        }
    }, []);

    const resetViewerState = useCallback(() => {
        setViewerPhase('closed');
        setSelectedIndex(null);
        setTransitionImageUrl(null);
        setTransitionRect(null);
        setTransitionBorderRadius(THUMBNAIL_BORDER_RADIUS_PX);
        setBackdropOpacity(0);
    }, []);

    const registerThumbnailImage = useCallback((index: number, image: HTMLImageElement | null) => {
        if (image) {
            thumbnailImageRefs.current.set(index, image);
            return;
        }

        thumbnailImageRefs.current.delete(index);
    }, []);

    const loadNextPage = useCallback(async () => {
        if (!hasNext || !nextCursor || loadingMoreRef.current) {
            return { loadedCount: 0, unauthorized: false };
        }

        loadingMoreRef.current = true;
        setIsLoadingMore(true);

        try {
            const thumbnailPageResponse = await getNextEventThumbnailUrls(publicEventId, nextCursor);
            if (thumbnailPageResponse.code == 401) {
                router.push(`/e/${publicEventId}`);
                return { loadedCount: 0, unauthorized: true };
            }

            if (thumbnailPageResponse.code != 200 || !thumbnailPageResponse.data) {
                setHasNext(false);
                return { loadedCount: 0, unauthorized: false };
            }

            const nextPage = thumbnailPageResponse.data;
            setThumbnailUrls((currentUrls) => [...currentUrls, ...nextPage.items]);
            setHasNext(nextPage.hasNext);
            setNextCursor(nextPage.nextCursor);

            return { loadedCount: nextPage.items.length, unauthorized: false };
        } finally {
            loadingMoreRef.current = false;
            setIsLoadingMore(false);
        }
    }, [hasNext, nextCursor, publicEventId, router]);

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

                void loadNextPage();
            },
            {
                root: scrollContainerNode,
                rootMargin: "300px 0px"
            }
        );

        observer.observe(loadMoreNode);

        return () => observer.disconnect();
    }, [hasNext, isLoadingMore, loadNextPage, nextCursor]);

    const openPhotoViewer = useCallback((index: number) => {
        const selectedImage = thumbnailImageRefs.current.get(index);
        const photoUrl = thumbnailUrls[index];
        if (!photoUrl) {
            return;
        }

        clearAnimationState();
        setSelectedIndex(index);

        if (!selectedImage) {
            setViewerPhase('open');
            setBackdropOpacity(1);
            return;
        }

        const sourceRect = selectedImage.getBoundingClientRect();
        const targetRect = getViewerTargetRect(selectedImage);

        setViewerPhase('opening');
        setTransitionImageUrl(photoUrl);
        setTransitionRect({
            top: sourceRect.top,
            left: sourceRect.left,
            width: sourceRect.width,
            height: sourceRect.height
        });
        setTransitionBorderRadius(THUMBNAIL_BORDER_RADIUS_PX);
        setBackdropOpacity(0);

        animationFrameRef.current = window.requestAnimationFrame(() => {
            setTransitionRect(targetRect);
            setTransitionBorderRadius(VIEWER_BORDER_RADIUS_PX);
            setBackdropOpacity(1);
        });

        animationTimeoutRef.current = window.setTimeout(() => {
            setViewerPhase('open');
            setTransitionImageUrl(null);
            setTransitionRect(null);
            setTransitionBorderRadius(VIEWER_BORDER_RADIUS_PX);
            setBackdropOpacity(1);
        }, VIEWER_TRANSITION_MS);
    }, [clearAnimationState, thumbnailUrls]);

    const handleCloseViewer = useCallback(() => {
        if (selectedIndex == null || viewerPhase == 'closing-zoom' || viewerPhase == 'closing-fade' || viewerPhase == 'closed') {
            return;
        }

        clearAnimationState();

        const selectedImage = thumbnailImageRefs.current.get(selectedIndex);
        const photoUrl = thumbnailUrls[selectedIndex];
        if (!photoUrl) {
            resetViewerState();
            return;
        }

        if (!selectedImage) {
            setViewerPhase('closing-fade');
            setBackdropOpacity(0);
            animationTimeoutRef.current = window.setTimeout(() => {
                resetViewerState();
            }, VIEWER_TRANSITION_MS);
            return;
        }

        const destinationRect = selectedImage.getBoundingClientRect();
        if (!isRectVisibleInViewport(destinationRect)) {
            setViewerPhase('closing-fade');
            setBackdropOpacity(0);
            animationTimeoutRef.current = window.setTimeout(() => {
                resetViewerState();
            }, VIEWER_TRANSITION_MS);
            return;
        }

        const sourceRect = getViewerTargetRect(selectedImage);

        setViewerPhase('closing-zoom');
        setTransitionImageUrl(photoUrl);
        setTransitionRect(sourceRect);
        setTransitionBorderRadius(VIEWER_BORDER_RADIUS_PX);
        setBackdropOpacity(1);

        animationFrameRef.current = window.requestAnimationFrame(() => {
            setTransitionRect({
                top: destinationRect.top,
                left: destinationRect.left,
                width: destinationRect.width,
                height: destinationRect.height
            });
            setTransitionBorderRadius(THUMBNAIL_BORDER_RADIUS_PX);
            setBackdropOpacity(0);
        });

        animationTimeoutRef.current = window.setTimeout(() => {
            resetViewerState();
        }, VIEWER_TRANSITION_MS);
    }, [clearAnimationState, resetViewerState, selectedIndex, thumbnailUrls, viewerPhase]);

    const showPreviousPhoto = useCallback(async () => {
        if (viewerPhase != 'open' || selectedIndex == null || selectedIndex == 0) {
            return;
        }

        setSelectedIndex(selectedIndex - 1);
    }, [selectedIndex, viewerPhase]);

    const showNextPhoto = useCallback(async () => {
        if (viewerPhase != 'open' || selectedIndex == null || isLoadingMore) {
            return;
        }

        if (selectedIndex < thumbnailUrls.length - 1) {
            setSelectedIndex(selectedIndex + 1);
            return;
        }

        if (!hasNext) {
            return;
        }

        const loadResult = await loadNextPage();
        if (loadResult.unauthorized || loadResult.loadedCount == 0) {
            return;
        }

        setSelectedIndex((currentIndex) => currentIndex == null ? null : currentIndex + 1);
    }, [hasNext, isLoadingMore, loadNextPage, selectedIndex, thumbnailUrls.length, viewerPhase]);

    useEffect(() => {
        if (viewerPhase == 'closed' || selectedIndex == null) {
            return;
        }

        const handleKeyDown = (event: KeyboardEvent) => {
            if (event.key == 'Escape') {
                event.preventDefault();
                handleCloseViewer();
            }
            else if (event.key == 'ArrowLeft') {
                event.preventDefault();
                void showPreviousPhoto();
            }
            else if (event.key == 'ArrowRight') {
                event.preventDefault();
                void showNextPhoto();
            }
        };

        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [handleCloseViewer, selectedIndex, showNextPhoto, showPreviousPhoto, viewerPhase]);

    const focusedPhotoUrl = selectedIndex == null ? null : thumbnailUrls[selectedIndex] ?? null;
    const canNavigatePrev = viewerPhase == 'open' && selectedIndex != null && selectedIndex > 0;
    const canNavigateNext = viewerPhase == 'open' && selectedIndex != null && (selectedIndex < thumbnailUrls.length - 1 || hasNext);

    return (
        <>
           <AddMediaModal mode={photoMode} closeHandler={() => setPhotoMode(PhotoMode.Default)} /> 
           <PhotoViewer
                phase={viewerPhase}
                photoUrl={focusedPhotoUrl}
                canNavigatePrev={canNavigatePrev}
                canNavigateNext={canNavigateNext}
                onNavigatePrev={() => void showPreviousPhoto()}
                onNavigateNext={() => void showNextPhoto()}
                onClose={handleCloseViewer}
                transitionImageUrl={transitionImageUrl}
                transitionRect={transitionRect}
                transitionBorderRadius={transitionBorderRadius}
                backdropOpacity={backdropOpacity}
            />

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
                    <PhotoGallery
                        thumbnailUrls={thumbnailUrls}
                        openPhotoViewer={openPhotoViewer}
                        registerThumbnailImage={registerThumbnailImage}
                    /> 
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

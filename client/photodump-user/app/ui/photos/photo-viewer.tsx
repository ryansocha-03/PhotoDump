'use client'

import { Dialog, DialogPanel, DialogTitle } from "@headlessui/react";

export interface ViewerRect {
    top: number,
    left: number,
    width: number,
    height: number
}

export type PhotoViewerPhase = 'closed' | 'opening' | 'open' | 'closing-zoom' | 'closing-fade';

const iconButtonClassName = "flex items-center justify-center rounded-full border border-white/15 bg-white/10 text-white shadow-[0_20px_60px_rgba(0,0,0,0.35)] ring-1 ring-black/10 backdrop-blur-md transition hover:cursor-pointer hover:bg-white/18 hover:shadow-[0_24px_70px_rgba(0,0,0,0.42)] disabled:cursor-not-allowed disabled:opacity-30 disabled:hover:bg-white/10";

function CloseIcon() {
    return (
        <svg
            aria-hidden="true"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.8"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="h-5 w-5 md:h-6 md:w-6"
        >
            <path d="M6 6L18 18" />
            <path d="M18 6L6 18" />
        </svg>
    )
}

function ChevronLeftIcon() {
    return (
        <svg
            aria-hidden="true"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.8"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="h-7 w-7 md:h-8 md:w-8"
        >
            <path d="M15 18L9 12L15 6" />
        </svg>
    )
}

function ChevronRightIcon() {
    return (
        <svg
            aria-hidden="true"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.8"
            strokeLinecap="round"
            strokeLinejoin="round"
            className="h-7 w-7 md:h-8 md:w-8"
        >
            <path d="M9 18L15 12L9 6" />
        </svg>
    )
}

export default function PhotoViewer({
    phase,
    photoUrl,
    canNavigatePrev,
    canNavigateNext,
    onNavigatePrev,
    onNavigateNext,
    onClose,
    transitionImageUrl,
    transitionRect,
    transitionBorderRadius,
    backdropOpacity
}: {
    phase: PhotoViewerPhase,
    photoUrl: string | null,
    canNavigatePrev: boolean,
    canNavigateNext: boolean,
    onNavigatePrev: () => void,
    onNavigateNext: () => void,
    onClose: () => void,
    transitionImageUrl: string | null,
    transitionRect: ViewerRect | null,
    transitionBorderRadius: number,
    backdropOpacity: number
}) {
    const isOpen = phase != 'closed' && photoUrl != null;
    const viewerContentVisible = phase == 'open' || phase == 'closing-fade';

    return (
        <Dialog open={isOpen} onClose={onClose} className="relative z-[60]">
            <div className="fixed inset-0" aria-hidden="true">
                <div
                    className="absolute inset-0 bg-black transition-opacity duration-200 ease-out"
                    style={{ opacity: backdropOpacity }}
                />

                {transitionImageUrl && transitionRect &&
                    <img
                        src={transitionImageUrl}
                        alt=""
                        aria-hidden="true"
                        className="pointer-events-none fixed object-cover shadow-2xl"
                        style={{
                            top: `${transitionRect.top}px`,
                            left: `${transitionRect.left}px`,
                            width: `${transitionRect.width}px`,
                            height: `${transitionRect.height}px`,
                            borderRadius: `${transitionBorderRadius}px`,
                            transition: 'top 220ms ease, left 220ms ease, width 220ms ease, height 220ms ease, border-radius 220ms ease'
                        }}
                    />
                }
            </div>

            <div className="fixed inset-0 flex items-center justify-center p-4 md:p-8">
                <DialogPanel
                    className={`relative flex h-full w-full items-center justify-center transition duration-200 ease-out ${viewerContentVisible ? 'opacity-100 scale-100' : 'pointer-events-none opacity-0 scale-95'}`}
                >
                    <DialogTitle className="sr-only">Photo viewer</DialogTitle>

                    <button
                        type="button"
                        aria-label="Close photo viewer"
                        className={`${iconButtonClassName} absolute right-2 top-2 z-20 h-11 w-11 md:right-4 md:top-4 md:h-12 md:w-12`}
                        onClick={onClose}
                    >
                        <CloseIcon />
                    </button>

                    <div className="flex h-full w-full items-center gap-2 pt-16 md:gap-4 md:pt-20">
                        <div className="flex w-14 shrink-0 justify-center md:w-16">
                            <button
                                type="button"
                                aria-label="Previous photo"
                                className={`${iconButtonClassName} h-14 w-14 md:h-16 md:w-16`}
                                onClick={onNavigatePrev}
                                disabled={!canNavigatePrev}
                            >
                                <ChevronLeftIcon />
                            </button>
                        </div>

                        <div className="flex min-w-0 flex-1 items-center justify-center overflow-hidden">
                            {photoUrl &&
                                <img
                                    src={photoUrl}
                                    alt="Focused event photo"
                                    className="max-h-full max-w-full object-contain shadow-2xl"
                                />
                            }
                        </div>

                        <div className="flex w-14 shrink-0 justify-center md:w-16">
                            <button
                                type="button"
                                aria-label="Next photo"
                                className={`${iconButtonClassName} h-14 w-14 md:h-16 md:w-16`}
                                onClick={onNavigateNext}
                                disabled={!canNavigateNext}
                            >
                                <ChevronRightIcon />
                            </button>
                        </div>
                    </div>
                </DialogPanel>
            </div>
        </Dialog>
    )
}

'use client'

import { Dialog, DialogPanel, DialogTitle } from "@headlessui/react";

export interface ViewerRect {
    top: number,
    left: number,
    width: number,
    height: number
}

export type PhotoViewerPhase = 'closed' | 'opening' | 'open' | 'closing-zoom' | 'closing-fade';

const navigationButtonClassName = "flex h-14 w-14 items-center justify-center border border-white/40 bg-black/40 text-4xl text-white backdrop-blur-sm md:h-16 md:w-16";

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
                        className="absolute right-2 top-2 z-20 rounded-full border border-white/40 bg-black/40 px-4 py-2 text-2xl text-white backdrop-blur-sm hover:cursor-pointer md:right-4 md:top-4"
                        onClick={onClose}
                    >
                        ×
                    </button>

                    <div className="flex h-full w-full items-center gap-2 pt-16 md:gap-4 md:pt-20">
                        <div className="flex w-14 shrink-0 justify-center md:w-16">
                            <button
                                type="button"
                                aria-label="Previous photo"
                                className={`${navigationButtonClassName} ${canNavigatePrev ? 'hover:cursor-pointer' : 'cursor-not-allowed opacity-35'}`}
                                onClick={onNavigatePrev}
                                disabled={!canNavigatePrev}
                            >
                                ‹
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
                                className={`${navigationButtonClassName} ${canNavigateNext ? 'hover:cursor-pointer' : 'cursor-not-allowed opacity-35'}`}
                                onClick={onNavigateNext}
                                disabled={!canNavigateNext}
                            >
                                ›
                            </button>
                        </div>
                    </div>
                </DialogPanel>
            </div>
        </Dialog>
    )
}

'use client'

export default function PhotoGallery({
    mediaMetadata
}: {
    mediaMetadata: string[]
}) {

    return (
        <div className="grid grid-cols-3 gap-6">
            {
                mediaMetadata.length == 0
                ? <div>No event photos yet. Upload public photos to share with other guests.</div>
                : mediaMetadata.map((m, i) => (
                    <img
                        key={`thumbnail-${i}`}
                        src={m}
                    />
                ))
            }
        </div>
    )
}
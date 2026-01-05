export default function LoadingThreeDots({
    size
}: {
    size: number
}) {
    return (
        <div className="flex gap-1">
            <div className={`h-${size} w-${size} rounded-full bg-gray-400 transition animate-pulse`} />
            <div className={`h-${size} w-${size} rounded-full bg-gray-400 transition delay-400 animate-pulse`} style={{ animationDelay: '0.2s'}}/>
            <div className={`h-${size} w-${size} rounded-full bg-gray-400 transition delay-800 animate-pulse`} style={{ animationDelay: '0.4s'}} />
        </div>
    )
}
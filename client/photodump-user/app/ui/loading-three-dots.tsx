export default function LoadingThreeDots() {
    return (
        <div className="flex items-center gap-1 h-full">
            <div className={`h-2 w-2 rounded-full bg-gray-400 transition animate-pulse`} />
            <div className={`h-2 w-2 rounded-full bg-gray-400 transition animate-pulse`} style={{ animationDelay: '0.2s'}}/>
            <div className={`h-2 w-2 rounded-full bg-gray-400 transition animate-pulse`} style={{ animationDelay: '0.4s'}} />
        </div>
    )
}
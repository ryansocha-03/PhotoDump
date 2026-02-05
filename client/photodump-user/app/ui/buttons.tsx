import React from "react"

export function FilledButton({
    text,
    clickHandler
}: {
    text: string,
    clickHandler: (event: React.MouseEvent<HTMLButtonElement>) => void
}) {
    return (
        <button
            className="bg-(--foreground) text-(--background) rounded-full h-full w-full hover:cursor-pointer"
            onClick={clickHandler}
        >
            {text}
        </button>
    )
}

export function OutlinedButton({
    text,
    clickHandler
}: {
    text: string,
    clickHandler: (event: React.MouseEvent<HTMLButtonElement>) => void
}) {
    return (
        <button
            className="border border-(--foreground) rounded-full h-full w-full hover:cursor-pointer"
            onClick={clickHandler}
        >
            {text}
        </button>
    )
}
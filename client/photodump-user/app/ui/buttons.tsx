import React from "react"

export function FilledButton({
    text,
    clickHandler,
    disabled = false
}: {
    text: string,
    clickHandler: (event: React.MouseEvent<HTMLButtonElement>) => void,
    disabled?: boolean
}) {
    return (
        <button
            className="bg-(--foreground) text-(--background) rounded-full h-full w-full hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-45"
            onClick={clickHandler}
            disabled={disabled}
        >
            {text}
        </button>
    )
}

export function OutlinedButton({
    text,
    clickHandler,
    disabled = false
}: {
    text: string,
    clickHandler: (event: React.MouseEvent<HTMLButtonElement>) => void,
    disabled?: boolean
}) {
    return (
        <button
            className="border border-(--foreground) rounded-full h-full w-full hover:cursor-pointer disabled:cursor-not-allowed disabled:opacity-45"
            onClick={clickHandler}
            disabled={disabled}
        >
            {text}
        </button>
    )
}

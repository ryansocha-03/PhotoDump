'use client'

import { useState } from "react"
import { EventPasswordSubmission } from "../api/events/route";
import { useRouter } from "next/navigation";

export default function EventPassword({
    eventId
} : {
    eventId: string
}) {
    const [password, setPassword] = useState("");
    const router = useRouter();
    
    const handleSubmit = async () => {
        const authBody: EventPasswordSubmission = {
            eventPassword: password,
            eventPublicId: eventId
        }

        const authResponse = await fetch('/api/events',
            {
                method: 'POST',
                body: JSON.stringify(authBody)
            }
        )

        if (authResponse.ok) {
            router.push(`/e/${eventId}/guests`)
        }
        if (authResponse.status == 401) {
            console.log("Incorrect password")
        }
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key == 'Enter') {
            e.preventDefault();
            handleSubmit();
        }
    }

    return (
        <div className="flex flex-col gap-5">
            <input 
                type="text"
                placeholder="Enter password..."
                value={password}
                className="border rounded-md border-(--foreground) text-xl p-2 focus:outline-none"
                onKeyDown={handleKeyDown}
                onChange={(e) => setPassword(e.target.value)}
            />
            <button
                className="w-full rounded-md bg-(--foreground) text-(--background) text-xl hover:cursor-pointer"
                onClick={handleSubmit}
            >
                Go
            </button>
        </div>
    )
}
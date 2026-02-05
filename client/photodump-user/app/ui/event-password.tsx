'use client'

import { useState } from "react"
import { EventPasswordSubmission } from "../api/events/route";
import { useRouter } from "next/navigation";
import LoadingThreeDots from "./loading-three-dots";

const errorMessages: string[] = [
    "Incorrect Password.",
    "Password cannot be empty.",
    "Issue when logging in with password. Please try again."
]

export default function EventPassword({
    eventId
} : {
    eventId: string
}) {
    const [password, setPassword] = useState("");
    const [showErrorMessage, setShowErrorMessage] = useState(false);
    const [errorMessage, setErrorMessage] = useState(0);
    const [isLoading, setIsLoading] = useState(false);
    const router = useRouter();
    
    const handleSubmit = async () => {
        if (password.trim().length == 0) {
            setErrorMessage(1);
            setShowErrorMessage(true);
            return;
        }

        const authBody: EventPasswordSubmission = {
            eventPassword: password,
            eventPublicId: eventId
        }

        setIsLoading(true);

        const authResponse = await fetch('/api/events',
            {
                method: 'POST',
                body: JSON.stringify(authBody)
            }
        )

        if (authResponse.ok) {
            router.push(`/e/${eventId}/guests`)
        }
        else if (authResponse.status == 401) {
            setErrorMessage(0);
            setShowErrorMessage(true);
        }
        else {
            setErrorMessage(2);
            setShowErrorMessage(true);
        }

        setIsLoading(false);
    }

    const handleKeyUp = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key == 'Enter') {
            e.preventDefault();
            if (!isLoading)
                handleSubmit();
        }
    }

    return (
        <>
            <div className="flex flex-col gap-5">
                <input 
                    type="text"
                    placeholder="Enter password..."
                    value={password}
                    className="border rounded-md border-(--foreground) text-xl p-2 focus:outline-none"
                    onKeyUp={handleKeyUp}
                    onChange={(e) => setPassword(e.target.value)}
                />
                <button
                    className={`w-full rounded-md bg-(--foreground) text-(--background) text-xl ${!isLoading ? 'hover:bg-gray-400 hover:cursor-pointer' : ''} flex justify-center`}
                    onClick={handleSubmit}
                    disabled={isLoading}
                >
                    <div className="min-h-10 flex items-center">
                    { isLoading 
                        ? <LoadingThreeDots />
                        : 'Go'
                    }
                    </div>
                </button>
            </div>
            <div className={`text-center text-red-400 text-wrap text-medium text-lg ${showErrorMessage ? 'block' : 'hidden'}`}>
                {errorMessages[errorMessage]}
            </div>
        </>
   )
}
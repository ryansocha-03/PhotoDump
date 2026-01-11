'use client'

import { useEffect, useState } from "react"
import { EventGuest } from "../lib/event/types";
import { useRouter } from "next/navigation";
import { GuestSearchSubmission } from "../api/events/guests/route";
import LoadingThreeDots from "./loading-three-dots";
import { GuestLoginRequestModel } from "../api/events/guests/login/route";

const errorMessages: string[] = [ 
    "No guests for provided name."
];

export default function GuestListSearch({
    eventId
}: {
    eventId: string
}) {
    const [query, setQuery] = useState("");
    const [results, setResults] = useState<EventGuest[]>([]);
    const [loading, setLoading] = useState(false);
    const [loggingInGuest, setLoggingInGuest] = useState(false);
    const [selectedGuest, setSelectedGuest] = useState(0);
    const [open, setOpen] = useState(false);
    const [errorMessage, setErrorMessage] = useState(0);
    const [showErrorMessage, setShowErrorMessage] = useState(false);

    const router = useRouter();

    useEffect(() => {
        const fetchGuests = async () => {
            if (query.length < 3) {
                setResults([]);
                setOpen(false);
                return;
            }

            setLoading(true);

            const searchData: GuestSearchSubmission = {
                eventId: eventId,
                guestName: query
            };
            
            const searchResponse = await fetch("/api/events/guests",
                {
                    method: 'POST',
                    body: JSON.stringify(searchData),
                    headers: {
                        'Content-Type': 'application'
                    }
                }
            );

            setLoading(false)

            if (searchResponse.status == 401) {
                router.push(`/e/${eventId}`);
                return;
            }

            if (!searchResponse.ok) {
                setResults([]);
                setOpen(false);
            }

            const searchResults: EventGuest[] = await searchResponse.json();
            setResults(searchResults);
            setLoading(false);
            if (searchResults.length == 0) {
                setOpen(false);
                setErrorMessage(0);
                setShowErrorMessage(true);
            }
            else {
                setOpen(true);
            }
        }

        setShowErrorMessage(false);
        const debounce = setTimeout(fetchGuests, 300)

        return () => clearTimeout(debounce);
    }, [query]);

    const handleGuestSelect = async (guest: EventGuest, index: number) => {
        if (!loggingInGuest) {
            setSelectedGuest(index);

            const guestData: GuestLoginRequestModel = {
                eventId: eventId,
                guestId: guest.guestId
            };

            setLoggingInGuest(true);

            const guestUpgradeResponse = await fetch('/api/events/guests/login',
                {
                    method: 'POST',
                    body: JSON.stringify(guestData)
                }
            )

            setLoggingInGuest(false);
            
            if (!guestUpgradeResponse.ok)
                router.push(`/e/${eventId}`);
            else
                router.push(`/e/${eventId}/photos`);
        }
    }
    
    return (
        <>
            <div>
                <div className="relative border border-white rounded-sm focus-within:shadow-sm focus-within:shadow-white">
                    <input
                        type="text"
                        placeholder="Enter your name..."
                        value={query}
                        onChange={(e) => setQuery(e.target.value)}
                        className="border-none text-xl p-2 focus:outline-none"
                    />
                    <div className="h-full bg-(--background) absolute right-0 top-0 px-3">
                        { loading && 
                            <LoadingThreeDots />
                        }
                    </div>
                </div>
                {
                    open && results.length > 0 &&
                    <div className="bg-black">
                        {results.map((guest, index) => (
                            <div 
                                key={`event-guest-${index}`} 
                                className="relative p-2 bg-(--foreground) text-(--background) hover:cursor-pointer hover:bg-gray-400" 
                                onClick={() => handleGuestSelect(guest, index)}
                            >
                                {guest.guestName}
                                <div className="h-full absolute top-0 right-0 px-3 bg-transparent">
                                    { loggingInGuest && selectedGuest == index && <LoadingThreeDots /> }
                                </div>
                            </div>
                        ))}
                    </div>
                }
            </div>
            {showErrorMessage &&
                <div className="pt-2 text-red-400 text-lg">
                    {errorMessages[errorMessage]}
                </div>
            }
        </>
    )
}
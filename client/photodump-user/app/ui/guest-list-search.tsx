'use client'

import { useEffect, useState } from "react"
import { EventGuest } from "../lib/event/types";
import { useRouter } from "next/navigation";
import { GuestSearchSubmission } from "../api/events/guests/route";

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
    const [open, setOpen] = useState(false);
    const [errorMessage, setErrorMessage] = useState(0);
    const [showErrorMessage, setShowErrorMessage] = useState(false);

    const router = useRouter();
    const controller = new AbortController();

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

    const handleGuestSelect = (guest: EventGuest) => {
        router.push(`/e/${eventId}/photos`)
    }
    
    return (
        <>
            <div>
                <input
                    type="text"
                    placeholder="Enter your name..."
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    className="border border-white text-xl p-2 focus:outline-none"
                />
                {
                    open && results.length > 0 &&
                    <div className="bg-black">
                        {results.map((guest, index) => (
                            <div 
                                key={`event-guest-${index}`} 
                                className="p-2 bg-(--foreground) text-(--background) hover:cursor-pointer hover:bg-gray-400" 
                                onClick={() => handleGuestSelect(guest)}
                            >
                                {guest.guestName}
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
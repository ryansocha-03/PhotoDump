'use client'

import { useEffect, useState } from "react"
import { EventGuest } from "../lib/event/types";
import { useRouter } from "next/navigation";

export default function GuestListSearch({
    eventId
}: {
    eventId: string
}) {
    const [query, setQuery] = useState("");
    const [results, setResults] = useState<EventGuest[]>([]);
    const [loading, setLoading] = useState(false);
    const [open, setOpen] = useState(false);

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
            const searchResponse = await fetch(`/api/events/guests?name=${query}`)
            if (!searchResponse.ok) {
                setResults([]);
                setOpen(false);
            }
            const searchResults: EventGuest[] = await searchResponse.json();
            setResults(searchResults);
            searchResults.length > 0 ? setOpen(true) : setOpen(false);
            console.log(searchResults);
            setLoading(false);
        }

        const debounce = setTimeout(fetchGuests, 3000)

        return () => clearTimeout(debounce);
    }, [query]);

    const handleGuestSelect = (guest: EventGuest) => {
        router.push(`/e/${eventId}/photos`)
    }
    
    return (
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
                            {guest.name}
                        </div>
                    ))}
                </div>
            }
        </div>
    )
}
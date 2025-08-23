'use client'

import { Button, Combobox, ComboboxInput, ComboboxOption, ComboboxOptions } from "@headlessui/react";
import { Guest } from "@/features/guests/types";
import { useState } from "react";

export default function GuestSelector({guests}: {guests: Guest[]}) {
    const [selectedGuest, setSelectedGuest] = useState<Guest | null>(null);
    const [query, setQuery] = useState('');
    const [showSelectGuestError, setShowSelectGuestError] = useState(false);

    const filteredGuests = query === ''
      ? guests
      : guests.filter((guest) => {
          return guest.name.toLowerCase().includes(query.toLowerCase())
        })
    
    const loginGuest = () => {
        if (!selectedGuest) {
            setShowSelectGuestError(true);
        }
        else {
            return
        }

    }

    return (
        <div className="flex flex-col items-center justify-center">
            <div className={`text-[#FF0000] ${showSelectGuestError ? 'block' : 'hidden'}`}>
                Please select your name from the guest list
            </div>

            <Combobox 
                immediate 
                value={selectedGuest} 
                onChange={(guest: Guest) => {
                    setSelectedGuest(guest);
                    setShowSelectGuestError(false);
                }}
                onClose={() => setQuery('')}
            >
                <ComboboxInput
                    displayValue={(guest: Guest) => guest ? guest.name : query}
                    onChange={(event) => setQuery(event.target.value)}
                    placeholder="What's your name?"
                    className={"border border-black m-2 w-100"}
                />
                <ComboboxOptions anchor="bottom" className={'w-(--input-width)'}>
                    {filteredGuests.map((guest) => (
                        <ComboboxOption key={guest.id} value={guest} className={'bg-white hover:bg-(--foreground) hover:cursor-pointer hover:text-white'}>
                            {guest.name}
                        </ComboboxOption>
                    ))}
                </ComboboxOptions>
            </Combobox>

            <Button
                className={'border border-(--foreground) rounded-full bg-(--foreground) text-white h-10 w-60 hover:cursor-pointer hover:bg-(--foregrounddark) hover:border-(--foregrounddark)'}
                onClick={loginGuest}
            >
                Continue
            </Button>
        </div>
    )
}
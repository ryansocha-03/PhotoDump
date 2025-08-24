'use client'

import { Button, Combobox, ComboboxInput, ComboboxOption, ComboboxOptions } from "@headlessui/react";
import { Guest } from "@/features/guests/types";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getGuestList } from "../data";

export default function GuestSelector({guestList} : {guestList: Guest[]}) {
    const router = useRouter();

    const [selectedGuest, setSelectedGuest] = useState<Guest | null>(null);
    const [query, setQuery] = useState('');
    const [showError, setShowError] = useState(false);
    const [errorType, setErrorType] = useState(0);

    const filteredGuests = query === ''
      ? guestList
      : guestList.filter((guest) => {
          return guest.name.toLowerCase().includes(query.toLowerCase())
        })
    
    const loginGuest = async () => {
        if (!selectedGuest) {
            setErrorType(0);
            setShowError(true);
        }
        else {
            const loginResponse = await fetch('/api/login', {
                method: 'POST',
                body: JSON.stringify(selectedGuest)
            })

            if (!loginResponse.ok) {
                setErrorType(1);
                setShowError(true);
            }
            else {
                router.push('/home');
            }
        }
    }

    return (
        <div className="flex flex-col items-center justify-center">
            <div className={`text-[#FF0000] ${showError ? 'block' : 'hidden'}`}>
               {errorType == 0 ? 'Please select your name from the guest list' : 'There was an issue when logging you in. Please contact Ryan Socha'}
            </div>

            <Combobox 
                immediate 
                value={selectedGuest} 
                onChange={(guest: Guest) => {
                    setSelectedGuest(guest);
                    setShowError(false);
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
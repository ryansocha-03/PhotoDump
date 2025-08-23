'use client'

import { Combobox, ComboboxInput, ComboboxOption, ComboboxOptions } from "@headlessui/react";
import { Guest } from "@/features/guests/types";
import { useState } from "react";

export default function GuestSelector({guests}: {guests: Guest[]}) {
    const [selectedGuest, setSelectedGuest] = useState(guests[0]);
    const [query, setQuery] = useState('');

    const filteredGuests = query === ''
      ? guests
      : guests.filter((guest) => {
          return guest.name.toLowerCase().includes(query.toLowerCase())
        })

    return (
        <Combobox value={selectedGuest} onChange={() => setSelectedGuest} onClose={() => setQuery('')}>
            <ComboboxInput
                displayValue={(guest: Guest) => guest.name}
                onChange={(event) => setQuery(event.target.value)}
            />
            <ComboboxOptions anchor="bottom">
                {filteredGuests.map((guest) => (
                    <ComboboxOption key={guest.id} value={guest}>
                        {guest.name}
                    </ComboboxOption>
                ))}
            </ComboboxOptions>
        </Combobox>
    )
}
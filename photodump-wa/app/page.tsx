import GuestSelector from "@/features/guests/components/guest-selector";
import { getGuestList } from "@/features/guests/data";
import { Button } from "@headlessui/react";

export default async function Home() {
  const guests = await getGuestList();

  return (
    <main>
      <div>
        <GuestSelector guests={guests} />
      </div>
    </main>
  );
}

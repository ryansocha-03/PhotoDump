import GuestSelector from "@/features/guests/components/guest-selector";
import { getGuestList } from "@/features/guests/data";

export default async function Home() {
  const guests = await getGuestList();

  return (
    <main>
      <div>
        <GuestSelector guestList={guests} />
      </div>
    </main>
  );
}

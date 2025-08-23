import GuestSelector from "@/features/guests/components/guest-selector";
import { getGuestList } from "@/features/guests/data";

export default async function Home() {
  const guests = await getGuestList();

  return (
    <main>
      <div className="flex justify-center">
        <GuestSelector guests={guests} />
      </div>
    </main>
  );
}

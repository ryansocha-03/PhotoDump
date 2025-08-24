import { Guest, GuestSchema } from "@/features/guests/types";

export async function getGuestList(): Promise<Guest[]> {
    const guestListResponse = await fetch(`${process.env.PHOTODUMP_API_URL}/guests`, {
        cache: 'no-store'
    })

    if (!guestListResponse.ok) {
        return [];
    }

    try {
        const guestData = await guestListResponse.json();
        return GuestSchema.array().parse(guestData);
    }
    catch (e) {
        return [];
    }
}
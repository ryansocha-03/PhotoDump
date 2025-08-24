import { Guest, GuestSchema } from "@/features/guests/types";
import { cookies } from "next/headers"
import { redirect } from "next/navigation";

export default async function Page() {
    const cookieStore = await cookies();

    let userSession: Guest;
    try {
        userSession = GuestSchema.parse(JSON.parse(atob(cookieStore.get('sid')?.value || '')));
    }
    catch (e) {
        console.log(e)
        redirect('/')
    }

    return (
        <div>{`Welcome ${userSession.name}`}</div>
    )
}
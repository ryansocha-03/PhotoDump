import { Guest, GuestSchema } from "@/features/guests/types";
import { NextRequest, NextResponse } from "next/server";

export async function POST(request: NextRequest) {
    let guest: Guest;
    try {
        const guestJson = await request.json();
        guest = GuestSchema.parse(guestJson);
    } catch (e) {
        console.log(e)
        return NextResponse.json({ error: "Invalid guest" }, { status: 400 })
    }
    const loggedInResponse = NextResponse.json({});
    loggedInResponse.cookies.set({
        name: 'sid', 
        value: btoa(JSON.stringify(guest)),
        httpOnly: true
    })
    return loggedInResponse;
}
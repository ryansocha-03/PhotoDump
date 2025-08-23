import { Guest } from "@/features/guests/types";
import { NextResponse } from "next/server";

export async function GET(): Promise<NextResponse<Guest[]>> {
    const mockedGuests: Guest[] = [
        { id: 1, name: "Amanda Socha"},
        { id: 2, name: "Joe Katic"},
        { id: 3, name: "Jeffrey Socha"},
        { id: 4, name: "Christine Socha"},
        { id: 5, name: "Jacob Socha"},
        { id: 6, name: "Ryan Socha"}
    ];

    return NextResponse.json(mockedGuests);
}
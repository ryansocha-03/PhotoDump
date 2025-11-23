import { EventGuest } from "@/app/lib/event/types";
import { mockGuestListSearch } from "@/app/mock-data/event-mock-data";
import { NextRequest, NextResponse } from "next/server";

export async function GET(request: NextRequest) : Promise<NextResponse<EventGuest[]>> {
    const searchQuery = request.nextUrl.searchParams.get('name');
    if (!searchQuery || searchQuery.trim().length < 3) {
        return NextResponse.json([]);
    }

    await setTimeout(() => {}, 3000)

    return NextResponse.json(mockGuestListSearch);
}
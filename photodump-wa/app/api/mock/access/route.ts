import { NextRequest, NextResponse } from "next/server";

export default function POST(request: NextRequest) {
    return NextResponse.json({ access: true });
}
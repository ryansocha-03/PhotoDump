import { NextRequest, NextResponse } from "next/server";

export default function middleware(request: NextRequest) {
    const sessionId = request.cookies.get('sid')?.value;
    if (!sessionId) {
        return NextResponse.redirect(new URL('/', request.url))
    }
}

export const config = {
    matcher: ['/((?!api|$|_next/static|_next/image|.*\\.png$).*)']
}
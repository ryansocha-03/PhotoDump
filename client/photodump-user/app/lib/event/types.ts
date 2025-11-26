export interface EventLandingData {
    id: number,
    eventPublicId: string,
    eventName: string,
    eventNameShort: string,
    colorPrimary: string,
    colorSecondary: string
}

export interface EventGuest {
    guestId: number,
    guestName: string
}
import { EventGuest, EventLandingData } from "../lib/event/types";

export let mockEventLanding: EventLandingData = {
    id: 1,
    eventPublicId: '',
    eventName: 'Jordan & Ryan',
    eventNameShort: 'J & E',
    colorPrimary: '#000000',
    colorSecondary: '#ffffff'
}

export let mockGuestListSearch: EventGuest[] = [
    {
        id: 0,
        name: 'Ryan Socha'
    },
    {
        id: 1,
        name: "Jacob Socha"
    },
    {
        id: 2,
        name: "Amanda Socha"
    }
];
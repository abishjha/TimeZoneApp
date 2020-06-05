export class TimeZone {
    id: number;
    name: string;
    city: string;
    differenceToGMT: number;
    userId: number;
    currTime?: string;
    differenceToBrowserTime?: number;
}
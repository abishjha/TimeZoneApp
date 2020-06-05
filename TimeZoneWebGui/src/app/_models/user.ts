import { TimeZone } from '.';

export class User {
    id: number;
    username: string;
    password: string;
    firstName: string;
    lastName: string;
    role?: string;
    token?: string;
    timeZones?: TimeZone[];
}
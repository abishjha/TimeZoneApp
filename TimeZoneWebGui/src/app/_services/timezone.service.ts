import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { TimeZone } from '@app/_models';
import { map } from 'rxjs/operators';
import { Time } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class TimeZoneService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<TimeZone[]>(`${environment.apiUrl}/timezones`)
            .pipe(map((data: TimeZone[]) => {
                return this.calculateRequiredFields(data);
            }));
    }

    addOrUpdate(userId: number, name: string, city: string, differenceToGMT: number, id?: number) {
        if(id) {
            return this.update(userId, name, city, differenceToGMT, id);
        } else {
            return this.register(userId, name, city, differenceToGMT);
        }
    }

    register(userId: number, name: string, city: string, differenceToGMT: number) {
        return this.http.post<any>(`${environment.apiUrl}/timezones`, {
            "userId": userId, 
            "name": name, 
            "city": city, 
            "differenceToGMT": differenceToGMT
        });
    }

    update(userId: number, name: string, city: string, differenceToGMT: number, id: number) {
        return this.http.put<any>(`${environment.apiUrl}/timezones/${id}`, {
            "userId": userId, 
            "name": name, 
            "city": city, 
            "differenceToGMT": differenceToGMT
        });
    }

    deleteOne(id: number) {
        return this.http.delete(`${environment.apiUrl}/timezones/${id}`);
    }

    calculateRequiredFields(timeZones: TimeZone[]) {
        return timeZones.map((timeZone) => {
            timeZone.currTime = this.calculateTimeInTimeZone(timeZone.differenceToGMT);
            timeZone.differenceToBrowserTime = this.calculateDifferenceToBrowserTime(timeZone.differenceToGMT);
            return timeZone;
        });
    }

    calculateTimeInTimeZone(diffToUtc: number) {
        let currTime = new Date();
        // get current time in UTC/GMT
        let currUtcTime = new Date(currTime.getUTCFullYear(), currTime.getUTCMonth(), currTime.getUTCDate(), currTime.getUTCHours(), currTime.getUTCMinutes(), currTime.getUTCSeconds());
        // calculate time in milliseconds in the time zone we need
        let timeInTimeZoneMilliseconds: number = currUtcTime.getTime() + (diffToUtc * 60 * 60 * 1000);
        // convert calculated milliseconds to a date object and get the time string
        return new Date(timeInTimeZoneMilliseconds).toLocaleTimeString();
    }

    calculateDifferenceToBrowserTime(diffToUtc: number) {
        let currTime = new Date();
        return diffToUtc + (currTime.getTimezoneOffset() / 60);
    }
}
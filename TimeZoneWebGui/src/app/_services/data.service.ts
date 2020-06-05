import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { TimeZone, User } from '@app/_models';
import { throwError, combineLatest, Observable, forkJoin, BehaviorSubject } from 'rxjs';
import { map, catchError, first } from 'rxjs/operators';
import { UserService } from './user.service';
import { TimeZoneService } from './timezone.service';
import { AuthenticationService } from './authentication.service';


/*
 * this class is responsible to get the data from the controllers and arrange them properly.
 * it arranges the data based on the logged in user's role:
 * ADMIN - gets all user and time zone data
 * MANAGER - gets all user data
 * USER - gets all time zone data for current user only
 *
 *
 * DATA MODEL => the data is an array of Users.  Every user has a property 'TimeZones' which is an 
 *        array of time zones for the given user.
 */



@Injectable({ providedIn: 'root' })
export class DataService {

    dataStream: BehaviorSubject<User[]> = new BehaviorSubject<User[]>(new Array());

    constructor(
        private userService: UserService,
        private timeZoneService: TimeZoneService,
        private authenticationService: AuthenticationService
    ) { }

    getDataStreamObservable() {
        return this.dataStream.asObservable();
    }

    getData() {
        this.getDataDependingOnRole().subscribe(
            data => {
                this.dataStream.next(data);
            },
            err => {
                console.log('error fetching data from api', err);
            }
        )
    }

    getDataDependingOnRole(): Observable<any> {
        if (this.authenticationService.currentUserValue.role === environment.role_admin) {
            return this.getAdminData();
        }
        else if (this.authenticationService.currentUserValue.role === environment.role_user_manager) {
            return this.getManagerData();
        }
        else {
            return this.getUserData();
        }
    }

    getAdminData() {
        // get the users and the time zones and combine the data into a meaningful structure
        return forkJoin<User[], TimeZone[]>([this.userService.getAll(), this.timeZoneService.getAll()])
            .pipe(first()).pipe(map(results => {
                let [users, timeZones] = results;
                console.log('extracted users and time zones', results);
                timeZones.forEach((el: TimeZone) => {
                    var idx = users.findIndex((obj: User) => obj.id == el.userId);
                    if (!users[idx].timeZones) { users[idx].timeZones = []; }
                    users[idx].timeZones.push(el);
                })
                return users;
            }));
    }

    getManagerData() {
        return this.userService.getAll().pipe(map(users => {
            console.log('extracted users', users);
            return users;
        }));
    }

    getUserData() {
        return this.timeZoneService.getAll().pipe(map(timeZones => {
            console.log('extracted timezone data', timeZones);
            var users = this.authenticationService.currentUserValue;
            users.timeZones = timeZones;
            return [users];
        }));
    }
}
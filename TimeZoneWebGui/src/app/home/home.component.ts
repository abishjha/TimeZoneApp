import { Component } from '@angular/core';
import { first } from 'rxjs/operators';
import { environment } from '../../environments/environment'

import { User, TimeZone } from '@app/_models';
import { DataService, AuthenticationService, TimeZoneService } from '@app/_services';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent {
    loading = false;
    originalUsers: User[];
    filteredUsers: User[];
    currentUser: User;
    dataUpdateInterval: any;

    searchword: string;

    constructor(
        private dataService: DataService,
        private timeZoneService: TimeZoneService,
        private authenticationService: AuthenticationService
    ) { }

    ngOnInit() {
        this.currentUser = this.authenticationService.currentUserValue;
        this.dataService.getDataStreamObservable().subscribe(result => {
            this.originalUsers = this.processData(result);
            this.filteredUsers = this.originalUsers;
            this.dataUpdateInterval = setInterval(this.updateDisplayedTime, environment.display_reset_duration);
            this.loading = false;
        });
        this.loadData();
    }

    loadData() {
        this.loading = true;
        if (this.dataUpdateInterval) {
            clearInterval(this.dataUpdateInterval);
        }
        this.dataService.getData();
    }

    // moves the current user to the beginning of the array for display purposes
    processData(users: User[]) {
        if (users.length <= 1) {
            return users;
        }

        for (let idx = 0; idx < users.length; idx++) {
            if (users[idx].id == this.currentUser.id) {
                [users[0], users[idx]] = [users[idx], users[0]];
                break;
            }
        }

        return users;
    }

    updateDisplayedTime = () => {
        for (let idx = 0; idx < this.filteredUsers.length; idx++) {
            if (this.filteredUsers[idx].timeZones) {
                this.filteredUsers[idx].timeZones = this.timeZoneService.calculateRequiredFields(this.filteredUsers[idx].timeZones);
            }
        }
    }

    searchThis() {
        if (!this.originalUsers) {
            return;
        }

        this.filteredUsers = new Array();

        for (let idx = 0; idx < this.originalUsers.length; idx++) {
            let user: User = {...this.originalUsers[idx]};
            if(user.timeZones) {
                user.timeZones = user.timeZones.filter(timeZone => timeZone.name.toLowerCase().includes(this.searchword.toLowerCase()));
            }

            if (user.username.toLowerCase().includes(this.searchword.toLowerCase()) || (user.timeZones && user.timeZones.length > 0)) {
                this.filteredUsers.push(user);
            }
        }
    }

    ngOnDestroy() {
        clearInterval(this.dataUpdateInterval);
    }
}
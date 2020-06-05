import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '@environments/environment';
import { User } from '@app/_models';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<User[]>(`${environment.apiUrl}/users`);
    }

    addOrUpdate(username: string, password: string, firstName: string, lastName: string, role: string, id?: number) {
        if(id) {
            return this.update(username, password, firstName, lastName, role, id);
        } else {
            return this.register(username, password, firstName, lastName, role);
        }
    }

    register(username: string, password: string, firstName: string, lastName: string, role: string) {
        return this.http.post<any>(`${environment.apiUrl}/users/register`, {
            "firstName": firstName, 
            "lastName": lastName,
            "username": username, 
            "password": password, 
            "role": role
        });
    }

    update(username: string, password: string, firstName: string, lastName: string, role: string, id: number) {
        return this.http.put<any>(`${environment.apiUrl}/users/${id}`, {
            "firstName": firstName, 
            "lastName": lastName,
            "username": username, 
            "password": password, 
            "role": role
        });
    }

    deleteOne(id: number) {
        return this.http.delete(`${environment.apiUrl}/users/${id}`);
    }
}
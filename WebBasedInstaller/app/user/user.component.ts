import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { IUser } from './user';
import { UserService } from './user.service';

@Component({
    selector: 'user-detail',
    templateUrl: 'app/user/user.component.html'
})

export class UserComponent implements OnInit {
    pageTitle: string = 'Current User';
    user: IUser;
    errorMessage: string;

    // Register the service
    constructor(private _userService: UserService) {
    }

    ngOnInit(): void {
        // Call the method that will call the service
        this.getCurrentUser();
    }

    getCurrentUser() {
        // Call the service
        this._userService.getCurrentUser().subscribe(
            user => this.user = user,
            error => this.errorMessage = <any>error);
    }
}
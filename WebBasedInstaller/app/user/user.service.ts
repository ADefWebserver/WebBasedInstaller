import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions, Request, RequestMethod, Headers } from '@angular/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { IUser } from './user';

@Injectable()
export class UserService {
    private _userUrl = 'odata/CurrentUser';

    constructor(private _http: Http) { }

    getCurrentUser(): Observable<IUser> {
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        // In this case we are not passing any parameters
        // so the { } has nothing inside
        return this._http.post(this._userUrl, { }, options)
            .map((response: Response) => <IUser[]>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        // in a real world app, we may send the server to some remote logging infrastructure
        // instead of just logging it to the console
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }
}
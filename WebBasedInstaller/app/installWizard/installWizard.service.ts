import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions, Request, RequestMethod, Headers } from '@angular/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

import { IVersion } from './version';
import { IStatus } from './status';
import { IConnectionSetting } from './connectionSetting';
import { IAuthentication } from './authentication';

@Injectable()
export class InstallWizardService {

    constructor(private _http: Http) { }

    getCurrentVersion(): Observable<IVersion> {
        var _Url = 'odata/CurrentVersion';
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        // In this case we are not passing any parameters
        // so the { } has nothing inside
        return this._http.post(_Url, { }, options)
            .map((response: Response) => <IVersion[]>response.json())
            .catch(this.handleError);
    }

    updateDatabase(): Observable<IStatus> {
        var _Url = 'odata/UpdateDatabase';
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        // In this case we are not passing any parameters
        // so the { } has nothing inside
        return this._http.post(_Url, {}, options)
            .map((response: Response) => <IStatus[]>response.json())
            .catch(this.handleError);
    }

    setConnection(ConnectionSetting: IConnectionSetting): Observable<string> {
        var _Url = 'odata/ODataConnectionSetting';
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        // passing the objConnectionSetting
        return this._http.post(_Url, JSON.stringify(ConnectionSetting), options)
            .map((response: Response) => <string>response.json().value)
            .catch(this.handleError);
    }

    createAdmin(Authentication: IAuthentication): Observable<string> {
        var Url = 'api/Angular2Admin/CreateAdminLogin';
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        return this._http.post(Url, JSON.stringify(Authentication), options)
            .map((response: Response) => <string>response.status.toString())
            .do(data => console.log('loginUser: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    loginAdmin(Authentication: IAuthentication): Observable<string> {
        var Url = 'api/Angular2Authentication/AdminLogin';
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        return this._http.post(Url, JSON.stringify(Authentication), options)
            .map((response: Response) => <string>response.status.toString())
            .do(data => console.log('loginUser: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    // Utility

    private handleError(error: Response) {
        // in a real world app, we may send the server to some remote logging infrastructure
        // instead of just logging it to the console
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }
}
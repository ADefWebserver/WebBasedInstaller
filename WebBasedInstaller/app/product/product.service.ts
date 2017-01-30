// Angular Imports
import {
    Injectable
} from '@angular/core';

import {
    Http, Response, RequestOptions,
    Request, RequestMethod, Headers
} from '@angular/http';

// Imports the product class used to map to the 
// OData DTOProduct class
import {
    IProduct
} from './product';

// This service uses rxjs Observable
import {
    Observable
} from 'rxjs/Observable';

import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

// This is marked Injectable because it will be 
// consumed in the Controller class
@Injectable()
export class ProductService {
    // This is the URL to the OData end point
    private _productUrl = 'odata/ODataProducts';

    // Pass the Http object to the class through the constructor
    constructor(private _http: Http) { }

    // ** Get all Products **
    getProducts(): Observable<IProduct[]> {
        // Make the Angular 2 Get
        // Note that we must use .value
        // because the OData response is wrapped in an object
        // and the data we want to map is at .value
        return this._http.get(this._productUrl)
            .map((response: Response) => <IProduct[]>response.json().value)
            .catch(this.handleError);
    }

    // ** Create a Product **
    createProduct(paramProduct: IProduct): Observable<IProduct> {
        // This is a Post so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Post
        return this._http.post(this._productUrl, JSON.stringify(paramProduct), options)
            .map((response: Response) => <IProduct>response.json())
            .catch(this.handleError);
    }

    // ** Update a Product **
    updateProduct(paramProduct: IProduct): Observable<void> {
        // This is a Put so we have to pass Headers
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        // Make the Angular 2 Put
        return this._http.put(
            this._productUrl + "(" + paramProduct.Id + ")",
            JSON.stringify(paramProduct), options)
            .catch(this.handleError);
    }

    // ** Delete a Product **
    deleteProduct(id: number): Observable<void> {
        // A Delete does not return anything
        return this._http.delete(this._productUrl + "(" + id + ")")
            .catch(this.handleError);
    }

    // ** Called when there are any errors **
    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }
}
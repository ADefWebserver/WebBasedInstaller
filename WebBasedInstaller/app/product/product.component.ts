import {
    Component, OnInit, OnDestroy, Input, Output,
    ViewContainerRef, EventEmitter, ViewChild, trigger
} from '@angular/core';
import {
    Router, ActivatedRoute
} from '@angular/router';
import {
    Subscription
} from 'rxjs/Subscription';
import {
    IProduct
} from './product';
import {
    ProductService
} from './product.service';
import {
    IUser
} from '../user/user';
import {
    UserService
} from '../user/user.service';
import {
    ModalDirective
} from 'ng2-bootstrap/ng2-bootstrap';
@Component({
    moduleId: module.id,
    selector: 'product-form',
    templateUrl: 'product.component.html'
})
export class ProductComponent implements OnInit {
    // Required for the Modal popup
    @ViewChild('childModal') public childModal: ModalDirective;
    // The current User
    user: IUser;
    // The currently selected Product
    productSelected: IProduct;
    // The list of Products
    products: IProduct[];
    // Any error messages
    errorMessage: string;
    // Controls if the Product list is to be shown
    showProductList: boolean = false;
    // Contructor is called when the class is created
    constructor(private _productService: ProductService,
        private _userService: UserService) {
        // Set ProductList to be hidden
        // Only show is user is logged in
        this.showProductList = false;
    }
    // Handle data related tasks in ngOnInit
    ngOnInit() {
        // Call the Current User method
        this.getCurrentUser();
        // Initialize productSelected
        this.productSelected = this.createNewProduct();
    }
    getCurrentUser() {
        // Call the getCurrentUser service
        this._userService.getCurrentUser().subscribe(
            user => {
                // Assign the user
                this.user = user;
                // See if we are logged in and have a user
                if (this.user.UserName != "[Not Logged in]") {
                    // Get the Products
                    this.getProducts();
                }
            },
            error => this.errorMessage = <any>error);
    }
    // ** Calls the service to retrieve all the Products **
    getProducts() {
        // Call the service
        this._productService.getProducts()
            .subscribe((products) => {
                // Set the products collection to the 
                // Products returned
                this.products = products;
                // Are any Products returned?
                if (products.length > 0) {
                    // Set the first Product as selected
                    this.productSelected = products[0];
                }
            },
            error => this.errorMessage = <any>error);

        // Show the Products list
        this.showProductList = true;
    }
    // ** Called when the Edit button is pressed ** 
    selectProduct(paramProduct: IProduct) {
        // Clear errorMessage
        this.errorMessage = "";

        // Set the selected Product
        this.productSelected = paramProduct;
        // Open the Popup
        this.childModal.config.backdrop = false // workaround for Angular bug
        this.childModal.show();
    }
    // ** Called when the Delete button is pressed **
    deleteProduct(paramProduct: IProduct) {
        // Clear errorMessage
        this.errorMessage = "";

        // Call the service to delete the Product
        this._productService.deleteProduct(paramProduct.Id)
            .subscribe(() => {
                // Refresh list - Get the Products
                this.getProducts();
            },
            error => this.errorMessage = <any>error);
    }
    // ** Called when the Add Product buton is pressed **
    newProduct() {
        // Set productSelected to a new Product
        this.productSelected = this.createNewProduct();

        // Open the Popup
        this.childModal.config.backdrop = false // workaround for Angular bug
        this.childModal.show();
    }
    // ** Called by the newProduct method to return 
    // a new Product **
    createNewProduct() {
        // Clear errorMessage
        this.errorMessage = "";

        // Create a new Product
        let newProduct: IProduct = {
            Id: 0,
            ProductName: '',
            ProductPrice: '',
        }
        return newProduct;
    }
    // ** Called when saving a Product **
    onSubmit() {
        // Clear errorMessage
        this.errorMessage = "";

        // Is this a new Product?
        if (this.productSelected.Id == 0) {
            // Call the service to Insert the Product
            this._productService.createProduct(this.productSelected)
                .subscribe((createdProduct) => {
                    // Add the Product to the collection
                    this.products.push(createdProduct);
                    // Close the Popup
                    this.childModal.hide();
                },
                error => this.errorMessage = <any>error);
        }
        else {
            // Call the service to update the Product
            this._productService.updateProduct(this.productSelected)
                .subscribe(() => {
                    // Close the Popup
                    this.childModal.hide();
                },
                error => this.errorMessage = <any>error);
        }
    }
}
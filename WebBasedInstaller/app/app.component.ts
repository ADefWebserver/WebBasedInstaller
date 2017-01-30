import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { AlertComponent } from 'ng2-bootstrap/ng2-bootstrap';

@Component({
    selector: 'my-app',
    template: `  
            <router-outlet></router-outlet>
     `
})
export class AppComponent implements OnInit {
    // Hack for root view container ref
    private viewContainerRef: ViewContainerRef;

    public constructor(viewContainerRef: ViewContainerRef) {
        // You need this small hack in order to catch application 
        // root view container ref
        this.viewContainerRef = viewContainerRef;
    }

    ngOnInit(): void {

    }
}
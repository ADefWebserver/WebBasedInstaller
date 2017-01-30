import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import {
    InputTextModule,
    DropdownModule,
    ButtonModule,
    FieldsetModule,
    TreeModule,
    TreeNode,
    SelectItem,
    TabMenuModule,
    MenuItem,
    TabViewModule,
    PanelModule,
    InputSwitchModule,
    PasswordModule    
} from 'primeng/primeng';

import { Ng2BootstrapModule } from 'ng2-bootstrap/ng2-bootstrap';
import { AppComponent } from './app.component';

import { UserService } from './user/user.service';
import { ProductService } from './product/product.service';
import { InstallWizardService } from './installWizard/installWizard.service';

import { InstallWizardComponent } from './installWizard/installWizard.component';
import { ProductComponent } from './product/product.component';
import { UserComponent } from './user/user.component';

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        FormsModule,
        InputTextModule,
        TreeModule,
        DropdownModule,
        ButtonModule,
        FieldsetModule,
        TabMenuModule,
        TabViewModule,
        PanelModule,
        InputSwitchModule,
        PasswordModule,
        Ng2BootstrapModule, 
        RouterModule.forRoot([
            { path: 'installWizard', component: InstallWizardComponent },
            { path: 'products', component: ProductComponent },
            { path: '', redirectTo: 'installWizard', pathMatch: 'full' },
            { path: '**', redirectTo: 'installWizard', pathMatch: 'full' } 
        ])
    ],
    declarations: [
        AppComponent,
        UserComponent,
        ProductComponent,
        InstallWizardComponent
    ],
    providers: [
        UserService,
        ProductService,
        InstallWizardService
    ],
    bootstrap: [
        AppComponent
    ]
})
export class AppModule { }

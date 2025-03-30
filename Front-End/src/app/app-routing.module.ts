import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductGridComponent } from './components/product-grid/product-grid.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { DeletedProductsComponent } from './components/deleted-products/deleted-products.component';

const routes: Routes = [
  { path: '', component: ProductGridComponent },
  { path: 'add', component: ProductFormComponent },
  { path: 'edit/:id', component: ProductFormComponent },
  { path: 'deleted-products', component: DeletedProductsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

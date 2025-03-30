import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from '../../product.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-product-grid',
  standalone: false,
  templateUrl: './product-grid.component.html',
  styleUrl: './product-grid.component.css',
})
export class ProductGridComponent implements OnInit {
  displayedColumns: string[] = ['name', 'description', 'price', 'actions'];
  totalRecords = 0;
  pageSize = 5;
  pageNumber = 1;
  dataSource: Product[] = [];
  showDeleted: boolean = false;
  loading = false;
  constructor(
    private productService: ProductService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
      this.productService
        .getProducts(this.pageNumber, this.pageSize)
        .subscribe({
          next: (response) => {
            this.dataSource = response.data;
            this.totalRecords = response.totalRecords;
            this.loading = false;
          },
          error: (error) => {
            console.error('Error loading products:', error);
            this.loading = false;
          },
        });
  }

  editProduct(id: string): void {
    this.router.navigate(['/edit', id]);
  }

  deleteProduct(product: Product): void {
    const oid = product.oid;
    if (
      confirm(
        'Are you sure you want to delete this product: ' + product.name + ' ?'
      )
    ) {
      this.productService.deleteProduct(oid).subscribe(() => {
        this.loadProducts();
      });
    }
  }
  toggleDeletedProducts(): void {
    this.showDeleted = !this.showDeleted;
    if (this.showDeleted) {
      this.router.navigate(['/deleted-products']);
    } else {
      this.router.navigate(['/']);
    }
  }
  showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['error-snackbar'],
    });
  }
  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }
}

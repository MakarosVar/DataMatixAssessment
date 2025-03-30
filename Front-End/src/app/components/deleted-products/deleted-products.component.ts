import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../product.service';
import { Product } from '../../product.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-deleted-products',
  templateUrl: './deleted-products.component.html',
  styleUrls: ['./deleted-products.component.css'],
  standalone: false,
})
export class DeletedProductsComponent implements OnInit {
  displayedColumns: string[] = ['name', 'description', 'price', 'actions'];
  deletedProducts: Product[] = [];
  loading = false;
  totalRecords = 0;
  pageSize = 5;
  pageNumber = 1;

  constructor(
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadDeletedProducts();
  }

  loadDeletedProducts(): void {
    this.loading = true;
    this.productService
      .getDeletedProducts(this.pageNumber, this.pageSize)
      .subscribe({
        next: (response) => {
          this.deletedProducts = response.data;
          this.totalRecords = response.totalRecords;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading deleted products:', error);
          this.loading = false;
        },
      });
  }

  restoreProduct(oid: string): void {
    this.productService.restoreProduct(oid).subscribe(() => {
      this.loadDeletedProducts();
    });
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
    this.loadDeletedProducts();
  }
}

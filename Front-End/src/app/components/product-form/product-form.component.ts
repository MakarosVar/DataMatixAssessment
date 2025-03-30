import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService, Product } from '../../product.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css'],
  standalone: false,
})
export class ProductFormComponent implements OnInit {
  productForm: FormGroup;
  productId: string = '';
  isEditing: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService
  ) {
    this.productForm = new FormGroup({
      name: new FormControl('', [Validators.required]),
      description: new FormControl('', [Validators.required]),
      price: new FormControl('', [Validators.required, Validators.min(0)]),
    });
  }

  ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id') || '';

    if (this.productId) {
      this.isEditing = true;
      this.loadProduct();
    }
  }

  loadProduct(): void {
    this.productService.getProductById(this.productId).subscribe({
      next: (product:Product) => {
        this.productForm.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
        });
      },
      error: (error:Error) => {
        console.error('Error loading product:', error);
      },
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      return;
    }

    const productData = this.productForm.value;

    if (this.isEditing) {
      this.productService.updateProduct(this.productId, productData).subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: (error: Error) => {
          console.error('Error updating product:', error);
        }
      });
    } else {
      this.productService.createProduct(productData).subscribe({

        next: () => {
          this.productForm.reset();
          this.isEditing = false;
          this.productId = '';
        },
        error: (error: Error) => {
          console.error('Error creating product:', error);
        }
      });
    }
  }
  goBack(): void {
    this.router.navigate(['/']);
  }
}



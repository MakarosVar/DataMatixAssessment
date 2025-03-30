import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  oid: string;
  name: string;
  description: string;
  price: number;
  createdAt: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'https://localhost:44349/api/products'; // Update with your API URL

  constructor(private http: HttpClient) {}

  getProducts(pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }
  getProductById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
  createProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }
  updateProduct(oid: string, product: Product): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${oid}`, product);
  }
  getDeletedProducts(pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/deleted?pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }
  restoreProduct(oid: string): Observable<any> {
    console.log('Restoring product with oid:', oid);
    return this.http.put(`${this.apiUrl}/restore/${oid}`, {});
  }
  deleteProduct(oid: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${oid}`);
  }
}

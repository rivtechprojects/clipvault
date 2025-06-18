import { Injectable, signal } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { ApiService } from './api.service';
import { Collection } from '../models/models';

export interface CreateCollectionRequest {
  name: string;
  description?: string;
  parentCollectionId?: number;
}

export interface UpdateCollectionRequest {
  name?: string;
  description?: string;
  parentCollectionId?: number;
}

@Injectable({ providedIn: 'root' })
export class CollectionsService {
  private collectionsSignal = signal<Collection[]>([]);
  collections = this.collectionsSignal.asReadonly();
  
  private loadingSignal = signal<boolean>(false);
  loading = this.loadingSignal.asReadonly();

  constructor(private apiService: ApiService) {}  fetchCollections(): Observable<Collection[]> {
    this.loadingSignal.set(true);
    
    return this.apiService.get<Collection[]>('collections').pipe(
      catchError(error => {
        console.error('Failed to fetch collections:', error);
        this.loadingSignal.set(false);
        return throwError(() => error);
      })
    );
  }
  loadCollections(): void {
    this.fetchCollections().subscribe({
      next: (collections) => {
        this.collectionsSignal.set(collections);
        this.loadingSignal.set(false);
      },
      error: (error) => {
        console.error('Error loading collections:', error);
        this.loadingSignal.set(false);
      }
    });
  }

  getCollectionById(id: number): Observable<Collection> {
    return this.apiService.get<Collection>(`collections/${id}`);
  }

  createCollection(collection: CreateCollectionRequest): Observable<Collection> {
    return this.apiService.post<Collection>('collections', collection);
  }

  updateCollection(id: number, collection: UpdateCollectionRequest): Observable<Collection> {
    return this.apiService.put<Collection>(`collections/${id}`, collection);
  }

  deleteCollection(id: number): Observable<void> {
    return this.apiService.delete<void>(`collections/${id}`);
  }

  addCollection(newCollection: Collection): void {
    const current = this.collectionsSignal();
    this.collectionsSignal.set([...current, newCollection]);
  }

  // Helper method to add collection with API call
  addNewCollection(name: string, parentId?: number): void {
    const request: CreateCollectionRequest = {
      name,
      parentCollectionId: parentId
    };

    this.createCollection(request).subscribe({
      next: (collection) => {
        this.addCollection(collection);
      },
      error: (error) => {
        console.error('Failed to create collection:', error);
      }
    });
  }
}
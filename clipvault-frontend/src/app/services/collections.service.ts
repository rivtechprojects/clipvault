import { Injectable, signal } from '@angular/core';
import { Collection } from '../models/models';
import { snippetCollectionsMock } from '../../Mocks/collections.mock';

@Injectable({ providedIn: 'root' })
export class CollectionsService {
  private collectionsSignal = signal<Collection[]>(snippetCollectionsMock);
  collections = this.collectionsSignal.asReadonly();

  constructor() {}

  fetchCollections() {
    // No-op for mock data
  }  

  addCollection(newCollection: Collection) {
    const current = this.collectionsSignal();
    this.collectionsSignal.set([...current, newCollection]);
  }
}
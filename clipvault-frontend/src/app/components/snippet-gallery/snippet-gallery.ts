import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit, Signal, effect } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CollectionsService } from '../../services/collections.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CollectionsListComponent } from '../collections-list/collections-list.component';
import { CodeEditor } from '../code-editor/code-editor';
import { Collection, Snippet } from '../../models/models';

@Component({
  selector: 'app-snippet-gallery',
  standalone: true,  imports: [
    CommonModule,
    CodeEditor,
    CollectionsListComponent,
    MatToolbarModule,
    MatListModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    FormsModule
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './snippet-gallery.html',
  styleUrl: './snippet-gallery.scss'
})
export class SnippetGallery implements OnInit {
  collectionsSignal: Signal<Collection[]>;
  loadingSignal: Signal<boolean>;
  selectedCollection: Collection | null = null;
  selectedSnippet: Snippet | null = null;
  errorMessage: string | null = null;
  constructor(private collectionsService: CollectionsService) {
    this.collectionsSignal = this.collectionsService.collections;
    this.loadingSignal = this.collectionsService.loading;
    this.selectedSnippet = null;
    
    // Use effect to reactively handle collections changes
    effect(() => {
      const collections = this.collectionsSignal();
      const isLoading = this.loadingSignal();
      
      // Only auto-select when we have collections and are not loading
      if (!isLoading && collections && collections.length > 0 && !this.selectedCollection) {
        this.selectedCollection = collections[0];
        if (collections[0].snippets && collections[0].snippets.length > 0) {
          this.selectedSnippet = collections[0].snippets[0];
        } else {
          this.selectedSnippet = null;
        }
      }
    });
  }

  ngOnInit() {
    this.loadCollections();
  }
  private loadCollections() {
    this.errorMessage = null;
    this.collectionsService.loadCollections();
    // The effect in the constructor will handle the selection automatically
  }

  onSelectCollection(collection: Collection) {
    this.selectedCollection = collection;
    if (collection && Array.isArray(collection.snippets) && collection.snippets.length > 0) {
      this.selectedSnippet = collection.snippets[0];
    } else {
      this.selectedSnippet = null;
    }
  }

  onAddCollection(name: string) {
    if (!name || !name.trim()) return;
    
    this.collectionsService.addNewCollection(name.trim());
  }

  selectSnippet(snippet: Snippet) {
    if (snippet && snippet.title && snippet.code !== undefined) {
      this.selectedSnippet = snippet;
    }
  }

  onRetryLoad() {
    this.loadCollections();
  }

  get isLoading(): boolean {
    return this.loadingSignal();
  }

  get collections(): Collection[] {
    return this.collectionsSignal();
  }
}

import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { Snippet } from '../snippet/snippet';
import { snippetCollectionsMock } from '../../../Mocks/snippet.mock';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CollectionsListComponent } from '../collections-list/collections-list.component';

@Component({
  selector: 'app-snippet-gallery',
  standalone: true,
  imports: [
    CommonModule,
    Snippet,
    CollectionsListComponent,
    MatToolbarModule,
    MatListModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    FormsModule
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './snippet-gallery.html',
  styleUrl: './snippet-gallery.scss'
})
export class SnippetGallery {
  collections = snippetCollectionsMock;
  selectedCollection = this.collections[0];
  selectedSnippet: any = this.selectedCollection.snippets[0];

  onSelectCollection(collection: any) {
    this.selectedCollection = collection;
    // Defensive: If the collection has snippets, select the first; otherwise, set to an empty object
    if (collection && Array.isArray(collection.snippets) && collection.snippets.length > 0) {
      this.selectedSnippet = collection.snippets[0];
    } else {
      this.selectedSnippet = {};
    }
  }

  onAddCollection(name: string) {
    if (!name || !name.trim()) return;
    this.collections.push({ name: name.trim(), snippets: [] });
  }

  selectSnippet(snippet: any) {
    // Only set selectedSnippet if the object has a title and code (i.e., is a snippet, not a collection)
    if (snippet && snippet.title && snippet.code !== undefined) {
      this.selectedSnippet = snippet;
    }
  }
}

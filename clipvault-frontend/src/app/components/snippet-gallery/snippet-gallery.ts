import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit, Signal } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { CollectionsService } from '../../services/collections.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CollectionsListComponent } from '../collections-list/collections-list.component';
import { CodeEditor } from '../code-editor/code-editor';
import { Collection, Snippet } from '../../models/models';

@Component({
  selector: 'app-snippet-gallery',
  standalone: true,
  imports: [
    CommonModule,
    CodeEditor,
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
export class SnippetGallery implements OnInit {
  collectionsSignal: Signal<Collection[]>;
  selectedCollection: Collection | null = null;
  selectedSnippet: Snippet | null = null;

  constructor(private collectionsService: CollectionsService) {
    this.collectionsSignal = this.collectionsService.collections;
    this.selectedSnippet = null;
  }

  ngOnInit() {
    this.collectionsService.fetchCollections();
    const collections = this.collectionsSignal();
    if (collections && collections.length > 0) {
      this.selectedCollection = collections[0];
      if (collections[0].snippets && collections[0].snippets.length > 0) {
        this.selectedSnippet = collections[0].snippets[0];
      } else {
        this.selectedSnippet = null;
      }
    }
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
    this.collectionsService.addCollection({
      id: Date.now(),
      name: name.trim(),
      snippets: [],
      parentCollectionId: null,
      subCollections: [],
      isDeleted: false
    });
  }

  selectSnippet(snippet: Snippet) {
    if (snippet && snippet.title && snippet.code !== undefined) {
      this.selectedSnippet = snippet;
    }
  }
}

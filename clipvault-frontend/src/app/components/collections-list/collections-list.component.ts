import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatNavList, MatListItem } from '@angular/material/list';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatLabel } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { NgTemplateOutlet } from '@angular/common';
import { Snippet } from '../../models/models';

@Component({
  selector: 'app-collections-list',
  standalone: true,
  imports: [
    MatNavList,
    MatListItem,
    MatIcon,
    MatButton,
    MatFormField,
    MatInput,
    MatLabel,
    FormsModule,
    NgTemplateOutlet
  ],
  templateUrl: './collections-list.component.html',
  styleUrls: ['./collections-list.component.scss']
})
export class CollectionsListComponent {
  @Input() collections: any[] = [];
  @Input() selectedCollection: any;
  @Output() selectCollection = new EventEmitter<any>();
  @Output() addCollection = new EventEmitter<string>();
  @Output() selectSnippet = new EventEmitter<any>();
  addCollectionPrompt = false;
  newCollectionName = '';
  selectedSnippet: Snippet | null = null;

  onAddCollection() {
    if (!this.newCollectionName || !this.newCollectionName.trim()) return;
    this.addCollection.emit(this.newCollectionName.trim());
    this.addCollectionPrompt = false;
    this.newCollectionName = '';
  }

  onSelectSnippet(snippet: any) {
    this.selectedSnippet = snippet;
    this.selectSnippet.emit(snippet);
  }

  // Recursively check if a collection has subCollections
  hasSubCollections(collection: any): boolean {
    return Array.isArray(collection.subCollections) && collection.subCollections.length > 0;
  }
}

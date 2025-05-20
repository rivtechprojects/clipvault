import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Snippet } from '../snippet/snippet';
import { snippetMock } from '../../../Mocks/snippet.mock';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-snippet-gallery',
  standalone: true,
  imports: [
    CommonModule,
    Snippet,
    MatToolbarModule,
    MatListModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './snippet-gallery.html',
  styleUrl: './snippet-gallery.scss'
})
export class SnippetGallery {
  snippets = snippetMock;
  expandedSnippet: any = null;

  expandSnippet(snippet: any) {
    this.expandedSnippet = snippet;
  }
  closeModal() {
    this.expandedSnippet = null;
  }
}

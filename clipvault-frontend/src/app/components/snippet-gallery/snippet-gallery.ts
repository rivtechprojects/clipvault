import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Snippet } from '../snippet/snippet';
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
  snippets = [
    {
      title: 'Async Await Example',
      language: 'javascript',
      code: 'async function fetchData() {\n  const res = await fetch(\'...\');\n  return res.json();\n}',
      tags: ['async', 'js', 'demo'],
      description: 'A simple async/await usage in JavaScript.'
    },
    {
      title: 'Python List Comprehension',
      language: 'python',
      code: '[x*x for x in range(10)]',
      tags: ['python', 'list', 'demo'],
      description: 'Quickly generate a list of squares in Python.'
    },
    {
      title: 'TypeScript Interface',
      language: 'typescript',
      code: 'interface User {\n  id: number;\n  name: string;\n}',
      tags: ['typescript', 'interface'],
      description: 'A basic TypeScript interface.'
    },
    // Add more demo snippets as needed
  ];
  expandedSnippet: any = null;

  expandSnippet(snippet: any) {
    this.expandedSnippet = snippet;
  }
  closeModal() {
    this.expandedSnippet = null;
  }
}

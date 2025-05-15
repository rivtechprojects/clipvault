import { Component } from '@angular/core';
import { MonacoCodeEditor } from '../monaco-code-editor/monaco-code-editor';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-snippet-gallery',
  imports: [MonacoCodeEditor, FormsModule],
  templateUrl: './snippet-gallery.html',
  styleUrl: './snippet-gallery.scss'
})
export class SnippetGallery {
  isAuthenticated = false;
  code = '';
  language = 'javascript';
}

import { Component, Input, Output, EventEmitter, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { NgStyle, CommonModule } from '@angular/common';
import { MonacoEditorModule } from 'ngx-monaco-editor';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { languageMock } from '../../../Mocks/language.mock';
import { NGX_MONACO_EDITOR_CONFIG, NgxMonacoEditorConfig } from 'ngx-monaco-editor';

@Component({
  selector: 'app-snippet',
  standalone: true,
  imports: [
    CommonModule, 
    NgStyle, 
    MatChipsModule, 
    MatIconModule, 
    MonacoEditorModule, 
    MatSelectModule, 
    FormsModule],
  templateUrl: './snippet.html',
  styleUrl: './snippet.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    {
      provide: NGX_MONACO_EDITOR_CONFIG,
      useValue: {
        baseUrl: 'assets'
      } as NgxMonacoEditorConfig
    }
  ]
})
export class Snippet {
  @Input() snippet: any;
  @Input() expanded = false;
  @Output() expand = new EventEmitter<void>();
  @Output() close = new EventEmitter<void>();

  languages = languageMock;

  onExpand() {
    this.expand.emit();
  }
  onClose() {
    this.close.emit();
  }
}

import { Component, Input, Output, EventEmitter, CUSTOM_ELEMENTS_SCHEMA, ViewChild, ElementRef } from '@angular/core';
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
    FormsModule
  ],
  templateUrl: './snippet.html',
  styleUrls: ['./snippet.scss'],
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

  @ViewChild('tagAddInput') tagAddInput: ElementRef<HTMLInputElement> | undefined;

  languages = languageMock;
  editTagIndex: number | null = null;
  editTagValue = '';
  editMode: boolean = false;

  toggleEditMode() {
    this.editMode = !this.editMode;
  }
  onExpand() {
    this.expand.emit();
  }
  onClose() {
    this.close.emit();
  }

  getLanguageLabel(langValue: string): string {
    const lang = this.languages?.find(l => l.value === langValue);
    return lang ? lang.label : langValue;
  }

  removeTag(tag: string) {
    if (!this.snippet?.tags) return;
    this.snippet.tags = this.snippet.tags.filter((t: string) => t !== tag);
  }

// Removed the unused addTagIfValid method.

  addAndEditTag() {
    if (!this.snippet.tags) this.snippet.tags = [];
    this.snippet.tags.push('');
    this.editTagIndex = this.snippet.tags.length - 1;
    this.editTagValue = '';
    setTimeout(() => {
      if (this.tagAddInput) {
        this.tagAddInput.nativeElement.value = '';
        this.tagAddInput.nativeElement.focus();
      }
    });
  }

  finishEditTag(index: number) {
    const value = this.editTagValue.trim();
    if (value && this.snippet.tags.filter((t: string, i: number) => t === value && i !== index).length === 0) {
      this.snippet.tags[index] = value;
    } else {
      this.snippet.tags.splice(index, 1);
    }
    this.editTagIndex = null;
    this.editTagValue = '';
  }

  trackByTag(index: number, tag: string) {
    return tag;
  }
}

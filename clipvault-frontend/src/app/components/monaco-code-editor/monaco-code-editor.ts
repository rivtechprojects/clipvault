import { Component, ElementRef, Input, Output, EventEmitter, ViewChild, AfterViewInit, OnDestroy, SimpleChanges, OnChanges } from '@angular/core';
import * as monaco from 'monaco-editor';

@Component({
  selector: 'app-monaco-code-editor',
  standalone: true,
  templateUrl: './monaco-code-editor.html',
  styleUrls: ['./monaco-code-editor.scss']
})
export class MonacoCodeEditor implements AfterViewInit, OnDestroy, OnChanges {
  @ViewChild('editorContainer', { static: true }) editorContainer?: ElementRef<HTMLDivElement>;
  @Input() value: string = '';
  @Input() language: string = 'javascript';
  @Input() readonly: boolean = false;
  @Output() valueChange = new EventEmitter<string>();

  private editorInstance?: monaco.editor.IStandaloneCodeEditor;

  ngAfterViewInit() {
    if (!this.editorContainer) return;
    this.editorInstance = monaco.editor.create(this.editorContainer.nativeElement, {
      value: this.value,
      language: this.language,
      readOnly: this.readonly,
      theme: 'vs-dark',
      automaticLayout: true,
      minimap: { enabled: false }
    });
    this.editorInstance.onDidChangeModelContent(() => {
      if (this.editorInstance) {
        this.valueChange.emit(this.editorInstance.getValue());
      }
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['language'] && this.editorInstance) {
      const model = this.editorInstance.getModel();
      if (model) {
        monaco.editor.setModelLanguage(model, this.language);
      }
    }
  }

  ngOnDestroy() {
    this.editorInstance?.dispose();
  }
}

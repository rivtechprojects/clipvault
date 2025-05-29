import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CodeEditor } from './code-editor';

describe('Snippet', () => {
  let component: CodeEditor;
  let fixture: ComponentFixture<CodeEditor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CodeEditor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CodeEditor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

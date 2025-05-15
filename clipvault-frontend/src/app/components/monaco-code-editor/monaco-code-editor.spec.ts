import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonacoCodeEditor } from './monaco-code-editor';

describe('MonacoCodeEditor', () => {
  let component: MonacoCodeEditor;
  let fixture: ComponentFixture<MonacoCodeEditor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonacoCodeEditor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonacoCodeEditor);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

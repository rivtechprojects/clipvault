import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnippetForm } from './snippet-form';

describe('SnippetForm', () => {
  let component: SnippetForm;
  let fixture: ComponentFixture<SnippetForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SnippetForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SnippetForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnippetGallery } from './snippet-gallery';

describe('SnippetGallery', () => {
  let component: SnippetGallery;
  let fixture: ComponentFixture<SnippetGallery>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SnippetGallery]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SnippetGallery);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

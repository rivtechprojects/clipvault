import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagInput } from './tag-input';

describe('TagInput', () => {
  let component: TagInput;
  let fixture: ComponentFixture<TagInput>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TagInput]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TagInput);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

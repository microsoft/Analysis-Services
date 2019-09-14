import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TreeControlContextMenuComponent } from './tree-control-context-menu.component';

describe('TreeControlContextMenuComponent', () => {
  let component: TreeControlContextMenuComponent;
  let fixture: ComponentFixture<TreeControlContextMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TreeControlContextMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TreeControlContextMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

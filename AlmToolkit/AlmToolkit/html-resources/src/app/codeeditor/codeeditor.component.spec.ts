import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CodeeditorComponent } from './codeeditor.component';

describe('CodeeditorComponent', () => {
    let component: CodeeditorComponent;
    let fixture: ComponentFixture<CodeeditorComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [CodeeditorComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(CodeeditorComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});

import { TestBed, inject } from '@angular/core/testing';

import { AppLogService } from './app-log.service';

describe('AppLogService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppLogService]
    });
  });

  it('should be created', inject([AppLogService], (service: AppLogService) => {
    expect(service).toBeTruthy();
  }));
});

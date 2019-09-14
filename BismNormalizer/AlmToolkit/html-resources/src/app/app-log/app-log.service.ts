import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable()
export class AppLogService {

  private messages: string[] = [];

  /**
   * Get the entire data in the heirarchical format
   * @param message - Message to be logged
   * @param entryType - Can be one of the following: Warn: Log as warning; Error: Log as error; Info: Log as info. Defaults to info
   */
  add(message: string, entryType: string) {
    this.messages.push(message);

    if (!environment.production) {
      entryType = entryType ? entryType.toLowerCase() : '';

      switch (entryType) {
        case 'warn':
          console.warn(message);
          break;
        case 'error':
          console.error(message);
          break;
        case 'info':
        default:
          console.log(message);
      }

    }
  }

  /**
   * Clear the message log
   */
  clear() {
    this.messages = [];
    if (!environment.production) {
      console.clear();
    }
  }

}

import { Injectable } from '@angular/core';

/**
 * Return the instance of window
 */
function _window(): Window {
    return window;
}

@Injectable()
export class WindowReferenceService {

    get nativeWindow(): Window {
        return _window();
    }

}

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { from } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { AppLogService } from '../app-log/app-log.service';
import { WindowReferenceService } from './window-reference.service';

import { ComparisonNode } from '../shared/model/comparison-node';
declare global {
  interface Window {
    comparisonJSInteraction: ComparisonJSInteraction;
  }
}
interface ComparisonJSInteraction {
  getComparisonList: () => Promise<string>;
  changeOccurred: (id: number, newAction: string, oldAction: string) => void;
  saveOrCompare(action: string) : void;
  performActionsOnSelectedActions(action: string,  selectedNodes: number[]) : void;
}
@Injectable()
export class GridDataService {

  private _window: Window;
  private databaseObjects!: ComparisonNode[];

  constructor(private logService: AppLogService, private windowRef: WindowReferenceService) {
    this._window = this.windowRef.nativeWindow;
  }

  /**
   * Get the data from the C# application
   */
  // getGridDataToDisplay(): Observable<ComparisonNode[]> {
  //   this.logService.add('Grid data service: Getting data from C#', 'info');
  //   return fromPromise(this._window['comparisonJSInteraction']
  //      .getComparisonList())
  //      .pipe(map((data: string) => JSON.parse(data)));
  //   //var test: Observable<ComparisonNode[]> = of(DatabaseSourceData);
  //   //return test;
  // }

  getGridDataToDisplay(): Observable<ComparisonNode[]> {
    this.logService.add('Grid data service: Getting data from C#', 'info');
    return from(this._window.comparisonJSInteraction.getComparisonList())
      .pipe(map((data: string) => JSON.parse(data) as ComparisonNode[]));
  }
  /**
   * Send the change done to the C# application
   * @param id - Id of the node for which action was changed
   * @param newAction - The updated action
   * @param oldAction - The old action that was selected
   */
  sendChange(id: number, newAction: string, oldAction: string): void {
    this.logService.add('Grid data service: Updating C# object on change in element', 'info');
    this._window['comparisonJSInteraction'].changeOccurred(id, newAction, oldAction);
  }

  saveOrCompare(action: string) {
    this.logService.add('Grid data service: Calling C# service to take action', 'info');
    this._window['comparisonJSInteraction'].saveOrCompare(action);
  }


  /**
   * Send the selected action and status to C# application
   * @param action - Action to be performed
   * @param status - Status of nodes for which the action is to be performed
   * @param selectedNodes - List of node Ids which are selected
   */
  sendSelectedNodesAndAction(action: string, selectedNodes: number[]) {
    this.logService.add('Grid data service: Sending the selected nodes and the action to be performed to C#', 'info');
    this._window['comparisonJSInteraction'].performActionsOnSelectedActions(action, selectedNodes);
  }
}

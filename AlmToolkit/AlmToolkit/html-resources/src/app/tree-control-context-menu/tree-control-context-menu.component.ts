import { Component, OnInit, Input } from '@angular/core';
import { GridDataService } from '../service/grid-data.service';
import { AppLogService } from '../app-log/app-log.service';

@Component({
  selector: 'app-tree-control-context-menu',
  templateUrl: './tree-control-context-menu.component.html',
  styleUrls: ['./tree-control-context-menu.component.css']
})
export class TreeControlContextMenuComponent implements OnInit {

  @Input() contextMenuPositionX = 0;
  @Input() contextMenuPositionY = 0;
  @Input() selectedNodes: number[] = [];
  @Input() selectedCell!: HTMLElement;
  constructor(private gridService: GridDataService, private appLog: AppLogService) { }

  ngOnInit() {
    document.getElementById('skip-selected')!.focus();
  }

  /**
   * When the element is hovered, focus on the same element
   * @param event - Event to get the target
   */
  focusElement(event: any) {
    event.preventDefault();
    if (event.target.classList && event.target.classList.contains('tree-control-context-menu-options')) {
      document.getElementById(event.target.id)!.focus();
    }
  }

  /**
   * Perform action on selected nodes based on action and status selected
   * @param action - the action to be performed
   * @param status - the status of objects for which action is to be performed
   */
  performAction(action: string) {
    this.gridService.sendSelectedNodesAndAction(action, this.selectedNodes);
  }

  /**
   * Handle key events on context menu
   * @param event - Take appropriate actions if key events are on context menu
   */
  onKeydown(event: any) {
    event.preventDefault();
    event.stopPropagation();
    let siblingRow!: HTMLElement |null;
    // This is for up and down arrow keys
    if (event.which === 38 || event.which === 40) {
      if (event.which === 38) {
        siblingRow = this.getSiblingElement(true, event.target.id) as HTMLElement |null;
      } else {
        siblingRow = this.getSiblingElement(false, event.target.id) as HTMLElement |null;
      }
      if (!siblingRow) {
        if (event.which === 38) {
          siblingRow = document.getElementById(event.target.id)!.parentElement!.lastElementChild as HTMLElement |null;
        } else {
          siblingRow = document.getElementById(event.target.id)!.parentElement!.firstElementChild  as HTMLElement |null;
        }
      }
      const allOptions = document.querySelectorAll('.tree-control-context-menu-options');
      let optionCounter;
      for (optionCounter = 0; optionCounter < allOptions.length; optionCounter += 1) {
        allOptions[optionCounter].classList.remove('hover');
      }
      siblingRow!.focus();
    } else if (event.which === 13) {
      // This is for selecting action when enter is pressed
      const action = document.getElementById(event.target.id)!.getAttribute('data-action');
      if (action) {
        this.performAction(action);
        document.getElementById(this.selectedCell.id)!.focus();
      }
    } else if (event.which === 27) {
      // This is to exit from context menu when ESC is pressed
      this.performAction('');
      document.getElementById(this.selectedCell.id)!.focus();
    }
  }

  /**
   * Get the sibling for the elements
   * @param prev - True if previous sibling is to be fetched and false if next sibling is to be fetched
   * @param id - Id of the element for which sibling is to be fetched
   */
  getSiblingElement(prev: boolean, id: string): Element|null {
    if (prev) {
      return document.getElementById(id)!.previousElementSibling;
    } else {
      return document.getElementById(id)!.nextElementSibling;
    }
  }
}

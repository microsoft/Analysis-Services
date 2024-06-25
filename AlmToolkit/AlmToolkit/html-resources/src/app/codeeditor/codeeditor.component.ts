import { Component, OnInit, Input, OnChanges } from '@angular/core';
import * as monaco from 'monaco-editor';
import { ComparisonNode } from '../shared/model/comparison-node';
import { lightPalette } from '../tmdl-utils/tmdl-color-palette';
import { getMonarchTokensDefinition } from "../tmdl-utils/tmdl-token-provider";
import { tmdlKewords, tmdlTypeKewords } from "../tmdl-utils/tmdl-types";
import { TmdlMonacoContributions } from '../tmdl-utils/tmdl.monaco.contributions';

@Component({
  selector: 'app-codeeditor',
  templateUrl: './codeeditor.component.html',
  styleUrls: ['./codeeditor.component.css']
})

export class CodeeditorComponent implements OnChanges {

  @Input() comparisonData!: ComparisonNode | null;
  public languageName: string = 'tmdl';
  constructor() { }

  ngOnChanges() {
    this.embedEditor();
  }

  /**
   * Embed the diff editor below the grid
   */
  embedEditor(): void {
    if (!this.comparisonData) {
      return;
    }

    TmdlMonacoContributions.registerLanguageContributions();

    const sourceDataModel = monaco.editor.createModel(this.comparisonData.SourceObjectDefinition, this.languageName);
    const targetDataModel = monaco.editor.createModel(this.comparisonData.TargetObjectDefinition, this.languageName);

    // If the container already contains an editor, remove it
    const codeEditorContainer = document.getElementById('code-editor-container');
    if (codeEditorContainer?.firstChild) {
      codeEditorContainer.removeChild(codeEditorContainer.firstChild);
    }

    // Create diff editor with required settings
    const diffEditor = monaco.editor.createDiffEditor(codeEditorContainer!, {

      scrollBeyondLastLine: false,
      automaticLayout: true,
      renderIndicators: false
    });

    // Set the original and modified code
    diffEditor.setModel({
      original: sourceDataModel,
      modified: targetDataModel
    });

    const monarchTokensProvider = getMonarchTokensDefinition(tmdlKewords, tmdlTypeKewords);
    monaco.languages.setMonarchTokensProvider(this.languageName, monarchTokensProvider);

    const tmdlLightTheme: monaco.editor.IStandaloneThemeData = {
      base: 'vs', // or 'vs' for light theme
      inherit: true, // If true, this theme inherits from other themes defined before it
      ...lightPalette,
    };
    monaco.editor.defineTheme('tmdlLightTheme', tmdlLightTheme);

    // Set the custom theme to the editor
    monaco.editor.setTheme('tmdlLightTheme');

  }
}

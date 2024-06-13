import { Component, OnInit, Input, OnChanges } from '@angular/core';
import * as monaco from 'monaco-editor';
import { ComparisonNode } from '../shared/model/comparison-node';
import { lightPalette } from '../tmdlColorPalette';
import { getMonarchTokensDefinition } from "../tmdl-TokenProvider";
import { tmdlKewords, tmdlTypeKewords } from "../tmdl-types";
import { TmdlLanguageId, TmdlMonacoContributions } from '../tmdl.monaco.contributions';
/*import { TMDLSemanticTokensProvider } from "../tmdl-semanticTokenProvider";*/

@Component({
  selector: 'app-codeeditor',
  templateUrl: './codeeditor.component.html',
  styleUrls: ['./codeeditor.component.css']
})

export class CodeeditorComponent implements OnChanges {

  @Input() comparisonData: ComparisonNode;
  public mode: string = 'tmdl';
  constructor() { }

  ngOnChanges(changes) {
    this.embedEditor();
  }

  /**
   * Embed the diff editor below the grid
   */
  embedEditor(): void {
    if (!this.comparisonData) {
      return;
    }

    if (this.mode === TmdlLanguageId) {
      TmdlMonacoContributions.registerLanguageContributions(this.mode);
    }

  
    const sourceDataModel = monaco.editor.createModel(this.comparisonData.SourceObjectDefinition, 'tmdl');
    const targetDataModel = monaco.editor.createModel(this.comparisonData.TargetObjectDefinition, 'tmdl');

    // If the container already contains an editor, remove it
    const codeEditorContainer = document.getElementById('code-editor-container');
    if (codeEditorContainer.firstChild) {
      codeEditorContainer.removeChild(codeEditorContainer.firstChild);
    }

    // Create diff editor with required settings
    const diffEditor = monaco.editor.createDiffEditor(codeEditorContainer, {

      scrollBeyondLastLine: false,
      automaticLayout: true,
      renderIndicators: false
    });

    // Set the original and modified code
    diffEditor.setModel({
      original: sourceDataModel,
      modified: targetDataModel
    });

    //const monarchTokensProvider = getMonarchTokensDefinition(tmdlKewords, tmdlTypeKewords);
    //monaco.languages.setMonarchTokensProvider("tmdl", monarchTokensProvider);
    const monarchTokensProvider = getMonarchTokensDefinition(tmdlKewords, tmdlTypeKewords);
    monaco.languages.setMonarchTokensProvider("tmdl", monarchTokensProvider);

    const tmdlLightTheme: monaco.editor.IStandaloneThemeData = {
      base: 'vs', // or 'vs' for light theme
      inherit: true, // If true, this theme inherits from other themes defined before it
      ...lightPalette,
    };
    console.log("Check if getting called again");
    monaco.editor.defineTheme('tmdlLightTheme', tmdlLightTheme);

    // Set the custom theme to the editor
    monaco.editor.setTheme('tmdlLightTheme');


    // Define the theme for original and modified code
    /* monaco.editor.defineTheme('flippedDiffTheme', {
      base: 'vs',
      inherit: true,
      rules: [],
      colors: {
        'diffEditor.insertedTextBackground': '#ff000033',
        'diffEditor.removedTextBackground': '#e2f6c5'
      }
    });
    monaco.editor.setTheme('flippedDiffTheme'); */
  }
}

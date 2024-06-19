//-----------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//        Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
import { IDisposable, editor } from "monaco-editor";

export class TmdlParserManager implements IDisposable {
  private disposables: IDisposable[] = [];

  constructor() { }

  public initialize(): void {
    this.disposables.push(editor.onDidCreateEditor(codeEditor => {
      this.disposables.push(codeEditor.onDidChangeModelContent(e => {
        this.handleModelChange(codeEditor);
      }));
    }));
  }

  public dispose(): void {
    for (let disposable of this.disposables) {
      disposable.dispose();
    }
  }

  private handleModelChange(codeEditor: editor.ICodeEditor): void {
    codeEditor.trigger("TriggerSuggestion", "editor.action.triggerSuggest", { auto: true, completionOptions: { providerFilter: undefined, kindFilter: undefined } });
  }
}

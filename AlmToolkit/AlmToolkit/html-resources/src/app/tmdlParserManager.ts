import { IDisposable, editor } from "monaco-editor";

export class TmdlParserManager implements IDisposable {
  private disposables: IDisposable[] = [];

  constructor() { }

  public initialize(languageId: string): void {
    this.disposables.push(editor.onDidCreateEditor(codeEditor => {
      this.disposables.push(codeEditor.onDidChangeModelContent(e => {
        this.handleModelChange(codeEditor, "reset");
      }));
    }));
  }

  public dispose(): void {
    for (let disposable of this.disposables) {
      disposable.dispose();
    }
  }

  private handleModelChange(codeEditor: editor.ICodeEditor, editSource: string): void {
    codeEditor.trigger("TriggerSuggestion", "editor.action.triggerSuggest", { auto: true, completionOptions: { providerFilter: undefined, kindFilter: undefined } });
  }
}

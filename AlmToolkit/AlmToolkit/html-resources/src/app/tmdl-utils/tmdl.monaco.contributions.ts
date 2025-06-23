//-----------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//        Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
import { IDisposable, languages } from 'monaco-editor/esm/vs/editor/editor.api';
import { TmdlParserManager } from './tmdl-parser-manager';

export const TmdlLanguageId = 'tmdl';

export interface TmdlContributions {
  tmdlParserManager: TmdlParserManager;
}

export class TmdlMonacoContributions {
  private static tmdlContributions: TmdlContributions;
  private static disposables: IDisposable[] = [];

  public static registerLanguageContributions(): TmdlContributions {

      TmdlMonacoContributions.registerTmdlContributions();
      TmdlMonacoContributions.initializeTmdlContributions();

      return this.tmdlContributions;

  }

  private static registerTmdlContributions(): void {
    languages.register({
      id: TmdlLanguageId,
      aliases: ['tmdl'],
      extensions: ['tmd', 'tmdl'],
    });
    const tmdlParserManager: TmdlParserManager = new TmdlParserManager();

    this.tmdlContributions = {
      tmdlParserManager
    };
  }

  private static initializeTmdlContributions() {
    this.disposables.push(languages.setLanguageConfiguration(TmdlLanguageId, {
    }));

    // Initialize the language providers
    this.tmdlContributions.tmdlParserManager.initialize();
  }
}


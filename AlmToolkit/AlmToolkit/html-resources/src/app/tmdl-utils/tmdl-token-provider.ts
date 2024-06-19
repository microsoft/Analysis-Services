//-----------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//        Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
import { languages } from "monaco-editor";

export interface MonarchTokenizer extends languages.IMonarchLanguage {
  tmdlKeywords: string[];
  tmdlTypeKeywords: string[];

}

/**
 * Creates Monarch tokens definition for a tmdl language.
 * @param tmdlKeywords Keywords specific to the language.
 * @param tmdlTypeKeywords Type keywords specific to the language.
 * @returns MonarchTokenizer object defining tokenization rules.
 */


export function getMonarchTokensDefinition(tmdlKeywords: string[], tmdlTypeKeywords: string[]): MonarchTokenizer {


  return {
    ignoreCase: true,
    tmdlKeywords: tmdlKeywords,
    tmdlTypeKeywords: tmdlTypeKeywords,

    tokenizer: {
      root: [
        { include: '@expression' }
      ],
      expression: [
        [/\/\*/, 'comment', '@comment'], // Start multi-line comment
        [/(\/\/).*/, 'comment'], // Single line comment
        [/(\w+)(\s*:)/, [{ token: 'variable' }, { token: 'operators', next: '@propertyValue' }]], //Properties
        [/^\s*\b\w+\b\s*$/, 'variable'], //Boolean properties
        [/(\w+)(\s+)(\w+)(\s+)(['"]?[\w-]+(?:\s+[\w-]+)*['"]?)$/, ['entity', '', 'identifier', '', 'meta']], // Reference objects
        [/(\w+)(\s+)('[\w\s.-:=]+'(?:\s*\.\s*'[\w\s.-:=]+')*|[\w-]+)$/, ['entity', '', 'identifier']], //Objects        
        [/(\w+)(\s+)(['"]?.+[\w-]+(?:\s+.+)*['"]?)(\s*=\s*)(.*)$/, ['entity', '', 'identifier', '', 'attribute']], //Partition
        [/(\w+)(\s+)(\w+)(\s*)(=)(\s*)([\[\{](?:"[^"]*"|\d+(?:\.\d+)*|[^[\]{}]+)*[\]\}]|\w+)/, ['entity', '', 'identifier', '', 'operators', '', 'attribute']], //Annotation
        [/(\w+)(\s+)(\w+)(\s*)(=)(\s*)(.*)/, ['entity', '', 'identifier', '', 'operators', 'attribute']],//Annotation
        [/(\w+)(\s*=\s*)(```)/, [{ token: 'variable' }, { token: 'operators' }, { token: 'attribute', next: '@exprWithBackticks' }]], //Expression enclosed with backticks (```)
        [/(\w+)(\s*=\s*\n)({([^{}]+)})/, ['variable', 'operators', 'attribute']],//JSON expressions
        [/(\w+)(\s*=)/, [{ token: 'variable' }, { token: 'operators', next: '@expr' }]], //Expressions not enclosed woth backticks (```) 
      ],
      comment: [
        [/[^/*]+/, 'comment'], // Anything besides * and / should be a comment
        [/\/\*/, 'comment', '@push'], // Nested comments
        [/\*\//, 'comment', '@pop'] // End comment
      ],
      expr: [
        [/^\s*/, 'attribute', '@pop'],
        [/s*(.*)/, 'attribute'],
      ],
      exprJSON: [
        [/[^{]+/, 'attribute'],
        [/(})/, 'attribute', '@pop'],
      ],
      exprWithBackticks: [
        [/[^```]+/, 'attribute'],
        [/(```)/, 'attribute', '@pop'],
      ],

      propertyValue: [
        { include: '@flowNumbers' },
        [
          /[\w-$()#.,;\\]+\s*/, // Match words followed by optional whitespace
          {
            cases: {
              '@tmdlTypeKeywords': 'keyword.type',
              '@tmdlKeywords': 'keyword',
              '@default': 'type'
            }
          }
        ],
        [
          /^/, // Match end of line
          {
            token: 'type',
            next: '@pop'
          } // Pop the state when moving to the next line
        ],

      ],
      flowNumbers: [
        { regex: /\b(?:0|[+-]?[0-9]+)(?![ \t]*\S+)\b/, action: { token: 'attribute' } }, // number
        { regex: /\b(?:0|[+-]?[0-9]+)(?:\.[0-9]+)?(?:e[-+][1-9][0-9]*)?\b/, action: { token: 'attribute' } }, // number.float
        { regex: /\b0o[0-7]+\b/, action: { token: 'attribute' } }, // number.octal
        { regex: /\b0x[0-9a-fA-F]+\b/, action: { token: 'attribute' } }, // number.hex
        { regex: /\b[+-]?\.(?:inf|Inf|INF)\b/, action: { token: 'attribute' } }, // number.infinity
        { regex: /\b\.(?:nan|Nan|NAN)\b/, action: { token: 'attribute' } }, // number.nan
        { regex: /\b\d{4}-\d\d-\d\d([Tt ]\d\d:\d\d:\d\d(\.\d+)?(( ?[+-]\d\d?(:\d\d)?)|Z)?)?\b/, action: { token: 'attribute' } }, // number.date
      ]
    }
  };
}

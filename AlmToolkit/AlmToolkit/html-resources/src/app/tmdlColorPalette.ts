export interface TMDLPalette {
  rules: { token: string; foreground: string }[];
  colors: { [key: string]: string };
}

export const lightPalette: TMDLPalette = {
  //rules: [
  //  { token: 'identifier', foreground: '#AF00DB' },
  //  { token: 'entity', foreground: '#7f0002' },
  //  { token: 'variable', foreground: '#001080' },
  //  { token: 'attribute', foreground: '#795E26' },


  //  { token: 'object', foreground: '#7f0002' },
  //  { token: 'identifier', foreground: '#AF00DB' },
  //  { token: 'property', foreground: '#001080' },
  //  { token: 'propertyvalue', foreground: '#267f99' },
  //  { token: 'string', foreground: '#267f99' },
  //  { token: 'boolean', foreground: '#001080' },
  //  { token: 'enum', foreground: '#0000ff' },
  //  { token: 'integer', foreground: '#795E26' },
  //  { token: 'number', foreground: '#795E26' },
  //  { token: 'expression', foreground: '#795E26' },

  //],
  //colors: {
  //  'editor.background': '#FFFFFF', // --colorNeutralBackground1 => --globalColorWhite
  //  'minimap.background': '#FFFFFF', // --colorNeutralBackground1 => --globalColorWhite
  //  'editorSuggestWidget.selectedBackground': '#D6EBFF',
  //  'editorSuggestWidget.selectedForeground': '#000000',
  //  'editorSuggestWidget.highlightForeground': '#0064BB',
  //  'editorSuggestWidget.focusHighlightForeground': '#357CC2',
  //}
  rules: [
    { token: 'identifier', foreground: 'af00db' },
    { token: 'entity', foreground: 'fa538e' },
    { token: 'variable', foreground: '001080' },
    { token: 'attribute', foreground: '795e26' },
    { token: 'keyword', foreground: 'db00d1' },
  ],
  colors: {
    'editor.background': '#ffffff',
    'minimap.background': '#ffffff',
    'editorsuggestwidget.selectedbackground': '#d6ebff',
    'editorsuggestwidget.selectedforeground': '#000000',
    'editorsuggestwidget.highlightforeground': '#0064bb',
    'editorsuggestwidget.focushighlightforeground': '#357cc2',
  }
};

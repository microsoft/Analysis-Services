//-----------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//        Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
export interface TMDLPalette {
  rules: { token: string; foreground: string }[];
  colors: { [key: string]: string };
}

export const lightPalette: TMDLPalette = {
  rules: [
    { token: "identifier", foreground: "af00db" },
    { token: "entity", foreground: "7f0002" },
    { token: "variable", foreground: "001080" },
    { token: "attribute", foreground: "795e26" },
  ],
  colors: {
    'diffEditor.insertedTextBackground': '#ff000033',
    'diffEditor.removedTextBackground': '#e2f6c5',
    "diffEditor.insertedLineBackground": "#ff000033",
    "diffEditor.removedLineBackground": "#e2f6c5",
  },
};

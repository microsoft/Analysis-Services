const path = require('path');
const MONACO_DIR = path.resolve(process.cwd(), 'node_modules/monaco-editor');
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

module.exports = {
  plugins: [new MonacoWebpackPlugin()],
 
  module: {
    rules: [
      {
        test: /\.ttf$/,
        include: MONACO_DIR,
        use: ['file-loader'],
      },
      {
        test: /\.css$/,
        include: MONACO_DIR,
        use: ['style-loader', 'css-loader'],
      },
    ],
  },
};

const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const project = require('./aurelia_project/aurelia.json');
const {AureliaPlugin} = require('aurelia-webpack-plugin');
const {optimize: {CommonsChunkPlugin, UglifyJsPlugin}, IgnorePlugin, ProvidePlugin} = require('webpack');
const {TsConfigPathsPlugin, CheckerPlugin} = require('awesome-typescript-loader');

// config helpers:
const ensureArray = (config) => config && (Array.isArray(config) ? config : [config]) || [];
const when = (condition, config, negativeConfig) =>
  condition ? ensureArray(config) : ensureArray(negativeConfig);

// primary config:
const title = 'Support Wheel Of Fate';
const outDir = path.resolve(__dirname, project.platform.output);
const srcDir = path.resolve(__dirname, 'src');
const nodeModulesDir = path.resolve(__dirname, 'node_modules');
const baseUrl = '/';

const cssRules = [
  {
    loader : 'css-loader',
    options: {
      minimize : true,
      sourceMap: false
    },
  },
];

const sassLoaders = [
  {
    loader : "css-loader",
    options: {
      minimize : true,
      sourceMap: false
    }
  },
  'resolve-url-loader',
  'sass-loader?sourceMap'];

module.exports = ({production, server, extractCss} = {}) => ({
  resolve  : {
    extensions: ['.ts', '.js'],
    modules   : [srcDir, 'node_modules'],
  },
  entry    : {
    app   : ['aurelia-bootstrapper'],
    vendor: ['bluebird', 'jquery', 'bootstrap-sass', 'whatwg-fetch'],
  },
  output   : {
    path             : outDir,
    publicPath       : baseUrl,
    filename         : production ? '[name].[chunkhash].bundle.js' : '[name].[hash].bundle.js',
    sourceMapFilename: production ? '[name].[chunkhash].bundle.map' : '[name].[hash].bundle.map',
    chunkFilename    : production ? '[name].[chunkhash].chunk.js' : '[name].[hash].chunk.js'
  },
  devServer: {
    contentBase       : outDir,
    // serve index.html for all 404 (required for push-state)
    historyApiFallback: true
  },
  devtool  : production ? 'nosources-source-map' : 'cheap-module-eval-source-map',
  module   : {
    rules: [
      // CSS required in JS/TS files should use the style-loader that auto-injects it into the website
      // only when the issuer is a .js/.ts file, so the loaders are not applied inside html templates
      {
        test  : /\.css$/i,
        issuer: [{not: [{test: /\.html$/i}]}],
        use   : extractCss ? ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use     : cssRules
        }) : ['style-loader', ...cssRules],
      },
      {
        test  : /\.css$/i,
        issuer: [{test: /\.html$/i}],
        // CSS required in templates cannot be extracted safely
        // because Aurelia would try to require it again in runtime
        use   : cssRules
      },
      {
        test  : /\.scss$/,
        use   : extractCss ? ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use     : sassLoaders,
        }) : ['style-loader', ...sassLoaders],
        issuer: /\.[tj]s$/i
      },
      {
        test  : /\.scss$/,
        use   : ['css-loader', 'sass-loader'],
        issuer: /\.html?$/i
      },
      {test: /\.html$/i, loader: 'html-loader'},
      {test: /\.ts$/i, loader: 'awesome-typescript-loader', exclude: nodeModulesDir},
      {test: /\.json$/i, loader: 'json-loader'},
      // use Bluebird as the global Promise implementation:
      {test: /[\/\\]node_modules[\/\\]bluebird[\/\\].+\.js$/, loader: 'expose-loader?Promise'},
      // embed small images and fonts as Data Urls and larger ones as files:
      {test: /\.(png|gif|jpg|cur)$/i, loader: 'url-loader', options: {limit: 8192}},
      {test    : /\.woff2(\?v=[0-9]\.[0-9]\.[0-9])?$/i,
        loader : 'url-loader',
        options: {limit: 10000, mimetype: 'application/font-woff2', outputPath: 'fonts/'}
      },
      {test    : /\.woff(\?v=[0-9]\.[0-9]\.[0-9])?$/i,
        loader : 'url-loader',
        options: {limit: 10000, mimetype: 'application/font-woff', outputPath: 'fonts/'}
      },
      // load these fonts normally, as files:
      {test: /\.(ttf|eot|svg|otf)(\?v=[0-9]\.[0-9]\.[0-9])?$/i, loader: 'file-loader'}
    ]
  },
  plugins  : [
    new AureliaPlugin(),
    new ProvidePlugin({
      'Promise'      : 'bluebird',
      '$'            : 'jquery',
      'jQuery'       : 'jquery',
      'window.jQuery': 'jquery'
    }),
    new TsConfigPathsPlugin(),
    new CheckerPlugin(),
    new IgnorePlugin(/^\.\/locale$/, /moment$/),
    new HtmlWebpackPlugin({
      template: 'index.ejs',
      metadata: {
        // available in index.ejs //
        title, server, baseUrl
      }
    }),
    ...when(extractCss, new ExtractTextPlugin({
      filename : production ? '[contenthash].css' : '[id].css',
      allChunks: true
    })),
    ...when(production, new CommonsChunkPlugin({
      name: ['common']
    })),
    ...when(production, new CopyWebpackPlugin([
      {from: 'static/favicon.ico', to: 'favicon.ico'}
    ])),
    ...when(production, new UglifyJsPlugin({
      sourceMap: true
    }))
  ]
});

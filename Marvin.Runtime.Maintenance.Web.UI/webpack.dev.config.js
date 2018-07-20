const webpack = require('webpack');
const merge = require('webpack-merge');
const baseConfig = require('./webpack.config.js');

module.exports = (env, options) => merge(baseConfig(env, options), {
    devtool: "inline-source-map",
    plugins: [
        new webpack.DefinePlugin({
            "RESTSERVER_PORT": "80",
          })
     ]
});

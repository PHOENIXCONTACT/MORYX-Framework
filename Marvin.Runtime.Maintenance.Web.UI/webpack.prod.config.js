const webpack = require('webpack');
const merge = require('webpack-merge');
const baseConfig = require('./webpack.config.js');

module.exports = (env, options) => merge(baseConfig(env, options), {
    plugins: [
        new webpack.DefinePlugin({
            "RESTSERVER_PORT": "window.location.port == '' ? \"80\" : window.location.port ",
          })
     ]
});
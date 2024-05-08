const webpack = require('webpack');
const { merge } = require('webpack-merge');
const baseConfig = require('./webpack.config.js');
const HtmlWebpackPlugin = require("html-webpack-plugin");

module.exports = (env, options) => merge(baseConfig(env, options), {
    devtool: "inline-source-map",
    devServer: {
        port: 4200
    },
    plugins: [
        new webpack.DefinePlugin({
            "BASE_URL": JSON.stringify('https://localhost:5000'),
        }),
        new HtmlWebpackPlugin({
            template: __dirname + '/src/index.html',
            filename: 'index.html',
            inject: 'body'
        })
    ],
});

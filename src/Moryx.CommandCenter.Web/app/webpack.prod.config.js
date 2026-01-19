/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

const webpack = require('webpack');
const { merge } = require('webpack-merge');
const baseConfig = require('./webpack.config.js');

module.exports = (env, options) => merge(baseConfig(env, options), {
    plugins: [
        new webpack.DefinePlugin({
            "BASE_URL": JSON.stringify(""),
        })
    ]
});

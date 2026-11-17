const path = require('node:path');
const { pathToFileURL } = require('node:url');

process.chdir(path.resolve(__dirname, '../backend'));

const backendTestPath = path.resolve(__dirname, '../backend/tests/unit-testing.js');

import(pathToFileURL(backendTestPath).href).catch((error) => {
    console.error(error);
    process.exitCode = 1;
});

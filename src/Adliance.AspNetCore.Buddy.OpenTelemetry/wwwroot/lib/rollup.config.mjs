import { nodeResolve } from "@rollup/plugin-node-resolve";
import commonjs from '@rollup/plugin-commonjs';
import json from "@rollup/plugin-json";
import terser from '@rollup/plugin-terser';

export default {
    input: 'telemetry.js',
    output: {
        file: './dist/telemetry.js',
        name: 'Adliance.Buddy',
        format: 'iife',
        globals: {
            perf_hooks: 'perf_hooks',
            url: 'url',
            http: 'http',
            https: 'https',
            zlib: 'zlib',
            stream: 'stream'
        }
    },
    plugins: [
        nodeResolve({
            main: true,
            browser: true
        }),
        commonjs(),
        json(),
        terser()
    ]
};

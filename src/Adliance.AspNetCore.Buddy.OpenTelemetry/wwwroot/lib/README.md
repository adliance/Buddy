# Packaging OpenTelemetry web dependencies

To avoid using CDN delivered dependencies, we combine the NPM packages in a self-contained script here.

The `telemetry.js` defines the necessary dependencies and a class for configuration in the client js.

## Packaging

- Update dependencies with `npm update --save`
- Install new dependencies with `npm install`
- Combine into distributable file with `npm run rollup:es`
- Publish new nuget package
- Add `dist/telemetry.js` to commit
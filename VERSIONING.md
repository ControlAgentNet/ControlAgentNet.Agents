# Versioning Strategy

This migration area uses one shared version across all foundational packages.

## Source of truth

`Directory.Build.props` contains `VersionPrefix`.

Everything else is derived from that base version.

## Rules

- local builds: `<VersionPrefix>-dev`
- pull requests: `<VersionPrefix>-preview.<run_number>`
- pushes to `main`: `<VersionPrefix>-alpha.<run_number>`
- tags like `v1.0.0`: exact release version `1.0.0`

## Release flow

This project follows the common open-source NuGet pattern:

1. `Directory.Build.props` remains the source of truth for `VersionPrefix`.
2. Pull requests validate the code and produce preview packages.
3. Pushes to `main` produce alpha packages.
4. Stable releases are created only from Git tags like `v0.1.0`.

To publish version `0.1.0`:

1. Set `<VersionPrefix>0.1.0</VersionPrefix>` in `Directory.Build.props`.
2. Merge the release-ready commit to `main`.
3. Create the tag:

```bash
git tag v0.1.0
git push origin v0.1.0
```

The CI workflow will then build, test, and pack version `0.1.0` exactly.

The workflow also validates that the tag matches `VersionPrefix`, so `v0.1.0` will fail if the repository still declares a different base version.

If the repository secret `NUGET_API_KEY` is configured with a valid NuGet.org API key, the same tag will also publish the generated packages automatically.

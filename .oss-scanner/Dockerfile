# syntax=docker/dockerfile:1
# Build environment for the Anthropic OSS Scanner (https://github.com/anthropics/oss-scanner).
# The image is built with the repository root as build context and network access. Everything after the build runs
# offline, so all NuGet packages and npm dependencies are restored here.
#
#   docker build -f .oss-scanner/Dockerfile -t moryx-oss .
#   docker run --rm -it --network none moryx-oss
FROM node:24-bookworm-slim AS node

FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
    DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=true \
    NUGET_XMLDOC_MODE=skip \
    MSBUILDDISABLENODEREUSE=1 \
    NG_CLI_ANALYTICS=false

RUN apt-get update && apt-get install -y --no-install-recommends \
      bash ca-certificates git git-lfs python3 sqlite3 \
    && rm -rf /var/lib/apt/lists/*

# Same Node.js major version as the CI (build-and-test.yml) for the Angular front-ends
COPY --from=node /usr/local/bin/node /usr/local/bin/node
COPY --from=node /usr/local/lib/node_modules /usr/local/lib/node_modules
RUN ln -s ../lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm \
    && ln -s ../lib/node_modules/npm/bin/npx-cli.js /usr/local/bin/npx \
    && node --version && npm --version

# scanner contract: the checkout lives inside the image, at /src
COPY . /src
WORKDIR /src

# Test content (e.g. src/Tests/TestContent/test.pdf) is stored in Git LFS
RUN git config --global --add safe.directory /src \
    && if [ -d .git ]; then git lfs install --local && git lfs pull || echo "WARNING: git lfs pull failed"; fi

RUN cat > /usr/local/bin/moryx-oss-build <<'SCRIPT' && chmod +x /usr/local/bin/moryx-oss-build
#!/usr/bin/env bash
# Offline rebuild of the solution or a single project: moryx-oss-build [path/to/project.csproj]
set -euo pipefail
cd /src
dotnet build "${1:-MORYX-Framework.slnx}" --no-restore --configuration Debug --disable-build-servers
SCRIPT

RUN cat > /usr/local/bin/moryx-oss-test <<'SCRIPT' && chmod +x /usr/local/bin/moryx-oss-test
#!/usr/bin/env bash
# Offline test run without rebuild: moryx-oss-test [path/to/test.csproj] [extra dotnet test args]
set -euo pipefail
cd /src
target="${1:-MORYX-Framework.slnx}"
shift || true
results="${MORYX_OSS_RESULTS:-/tmp/moryx-oss-results}"
mkdir -p "$results"
dotnet test "$target" --no-build --no-restore --configuration Debug \
    --results-directory "$results" --logger "trx" --blame-hang-timeout 5m "$@"
SCRIPT

# Debug on purpose: the BuildUI targets of the *.Web projects run `npm ci` and the Angular build only when the
# bundle in wwwroot/ is missing. It is created here, so later offline rebuilds skip npm entirely.
RUN dotnet restore MORYX-Framework.slnx \
    && dotnet build MORYX-Framework.slnx --no-restore --configuration Debug --disable-build-servers \
    && dotnet build-server shutdown

# Verify the image without failing the build on test failures
RUN moryx-oss-test || echo "WARNING: dotnet test reported failures"

CMD ["bash"]

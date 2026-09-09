#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

product_version="$PRODUCT_VERSION"
GLOBAL_GIT_PURL="pkg:git/${GITHUB_SERVER_URL#https://}/$GITHUB_REPOSITORY@${GITHUB_SHA:0:8}"
output_path=$(pwd)/sboms
mkdir -p "$output_path"

metadata_template_name="bom-metadata-template.xml"
metadata_name="bom-metadata.xml"

while IFS= read -r -d '' project
do
  echo ""
  echo "Checking project: $project"

  is_packable=$(dotnet msbuild "$project" -getProperty:IsPackable 2>/dev/null | tail -n 1 | tr '[:upper:]' '[:lower:]')
  is_deployable=$(dotnet msbuild "$project" -getProperty:CreateDeployment 2>/dev/null | tail -n 1 | tr '[:upper:]' '[:lower:]')

  if [ "$is_packable" != "true" ] && [ "$is_deployable" != "true" ]; then
    echo "Skipping non-packable and non-deployable project"
    continue
  fi

  dotnet_project_type=$(dotnet msbuild "$project" -getProperty:OutputType 2>/dev/null | tail -n 1 | tr '[:upper:]' '[:lower:]')

  project_dir=$(dirname "$project")
  project_dir="${project_dir#./}"
  project_name=$(basename "$project" .csproj)

  echo "Generating SBOMs for $project_name"

  output_dir="$output_path/$project_name"
  mkdir -p "$output_dir"

  PROJECT_GIT_PURL="$GLOBAL_GIT_PURL#$project_dir"
  PROJECT_NUGET_PURL="pkg:nuget/$project_name@$product_version"

  if [ "$dotnet_project_type" = "exe" ]; then
    echo "Detected project type for $project_name is application"
    export INNER_PROJECT_TYPE="application"
    export PROJECT_PURL="$PROJECT_GIT_PURL"
  else
    echo "Detected project type for $project_name is library"
    export INNER_PROJECT_TYPE="library"
    export PROJECT_PURL="$PROJECT_NUGET_PURL"
  fi
  export PROJECT_GIT_PURL

  if [[ -f "$project_dir/$metadata_template_name" ]]; then
    echo "Using project-specific $metadata_template_name"
    template_source="$project_dir/$metadata_template_name"
  else
    template_source="$SCRIPT_DIR/bom-metadata-template.xml"
    echo "Using default template from $template_source"
  fi
  envsubst < "$template_source" > "$project_dir/$metadata_name"
  cat "$project_dir/$metadata_name"

  #
  # .NET SBOM
  #
  dotnet-CycloneDX "$project" \
    -F Json \
    -ipr \
    -o "$output_dir" \
    -fn "bom-dotnet.json" \
    --set-version "$product_version" \
    --import-metadata-path "$project_dir/$metadata_name" \
    --disable-package-restore

  #
  # npm SBOMs (optional)
  #
  while IFS= read -r -d '' package_json
  do
    npm_dir=$(dirname "$package_json")
    echo "Found package.json for $project_name in $npm_dir"

    if [[ ! -f "$npm_dir/package-lock.json" ]]; then
      echo "Skipping npm SBOM for $project_name because package-lock.json is missing in $npm_dir"
      continue
    else
      echo "Found package-lock.json in $npm_dir"
    fi

    npm_relative_dir=${npm_dir#"$project_dir"}
    npm_relative_dir=${npm_relative_dir#/}

    if [ -z "$npm_relative_dir" ]; then
      npm_bom_name="bom-npm.json"
    else
      npm_bom_suffix=${npm_relative_dir//\//-}
      npm_bom_suffix=${npm_bom_suffix//[^[:alnum:]._-]/-}
      npm_bom_name="bom-npm-$npm_bom_suffix.json"
    fi

    pushd "$npm_dir"

    jq \
      --arg BUILD_VERSION "$product_version" \
      --arg project_name "$project_name.App" \
      '.version = $BUILD_VERSION | .name = $project_name' \
      package.json > package2.json
    mv package2.json package.json

    npm ci

    echo "Writing npm sbom to '$output_dir/$npm_bom_name'"
    cyclonedx-npm \
      --output-format JSON \
      --ignore-npm-errors \
      --output-file "$output_dir/$npm_bom_name"

    popd
  done < <(find "$project_dir" \
    -path "*/bin" -prune -o \
    -path "*/obj" -prune -o \
    -path "*/node_modules" -prune -o \
    -path "*/.git" -prune -o \
    -name "package.json" -print0)

  #
  # Merge .NET and npm SBOMs for the project
  #
  echo "Merging SBOMs for $project_name"

  mapfile -d '' bom_files < <(
    find "$output_dir" \
      -maxdepth 1 \
      -type f \
      -name "bom-*.json" \
      ! -name "bom-product.json" \
      -print0
  )

  if [ "${#bom_files[@]}" -eq 0 ]; then
    echo "No SBOM files found for $project_name"
  else
    echo "Merging ${#bom_files[@]} SBOM files for $project_name into bom-product.json"

    cyclonedx merge \
      --input-files "${bom_files[@]}" \
      --input-format json \
      --output-format json \
      --output-file "$output_dir/bom-product-raw.json" \
      --name "$project_name" \
      --version "$product_version"

    jq \
      --arg purl "$PROJECT_PURL" \
      --arg url "$GITHUB_SERVER_URL/$GITHUB_REPOSITORY" \
      --arg product_url "$GITHUB_SERVER_URL/$GITHUB_REPOSITORY/tree/$GITHUB_SHA/$project_dir" \
      --arg pipeline_url "$GITHUB_SERVER_URL/$GITHUB_REPOSITORY/actions/runs/$GITHUB_RUN_ID" \
      '.metadata.component.purl = $purl
      | .metadata.component.externalReferences[0] = {"type": "vcs", "url": $url, "comment": "Project reference"}
      | .metadata.component.externalReferences[1] = {"type": "vcs", "url": $product_url, "comment": "Folder view"}
      | .metadata.component.externalReferences[2] = {"type": "build-system", "url": $pipeline_url, "comment": "Build pipeline"}
      | .metadata.lifecycles = [{"phase": "build"}]
      ' \
      "$output_dir/bom-product-raw.json" > "$output_dir/bom-product.json"

    rm "$output_dir/bom-product-raw.json"
  fi
done < <(find . \
  -path "*/bin" -prune -o \
  -path "*/obj" -prune -o \
  -path "*/node_modules" -prune -o \
  -path "*/.git" -prune -o \
  -name "*.csproj" -print0)

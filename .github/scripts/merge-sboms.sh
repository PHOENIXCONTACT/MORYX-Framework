#!/usr/bin/env bash
set -euo pipefail

product_version="$PRODUCT_VERSION"
GLOBAL_GIT_PURL="pkg:git/${GITHUB_SERVER_URL#https://}/$GITHUB_REPOSITORY@${GITHUB_SHA:0:8}"
output_path=$(pwd)/sboms
project_name="${GITHUB_REPOSITORY##*/}"

mapfile -d '' product_boms < <(
  find "$output_path" \
    -mindepth 2 \
    -maxdepth 2 \
    -type f \
    -name "bom-product.json" \
    -print0
)

if [ "${#product_boms[@]}" -eq 0 ]; then
  echo "No product SBOMs found for global merge"
  exit 1
fi

enrich_global_sbom() {
  local input_file="$1"
  local output_file="$2"
  jq \
    --arg purl "$GLOBAL_GIT_PURL" \
    --arg url "$GITHUB_SERVER_URL/$GITHUB_REPOSITORY" \
    --arg product_url "$GITHUB_SERVER_URL/$GITHUB_REPOSITORY/tree/$GITHUB_SHA" \
    --arg pipeline_url "$GITHUB_SERVER_URL/$GITHUB_REPOSITORY/actions/runs/$GITHUB_RUN_ID" \
    '.metadata.component.purl = $purl
    | .metadata.component.externalReferences[0] = {"type": "vcs", "url": $url, "comment": "Project reference"}
    | .metadata.component.externalReferences[1] = {"type": "vcs", "url": $product_url, "comment": "Folder view"}
    | .metadata.component.externalReferences[2] = {"type": "build-system", "url": $pipeline_url, "comment": "Build pipeline"}
    | .metadata.lifecycles = [{"phase": "build"}]
    ' \
    "$input_file" > "$output_file"
}

if [ "${#product_boms[@]}" -eq 1 ]; then
  echo "Only one product SBOM found, enriching it as bom-all-products.json"
  enrich_global_sbom "${product_boms[0]}" "$output_path/bom-all-products.json"
  exit 0
fi

echo "Merging ${#product_boms[@]} product SBOMs into bom-all-products.json"

cyclonedx merge \
  --input-files "${product_boms[@]}" \
  --input-format json \
  --output-format json \
  --output-file "$output_path/bom-all-products-raw.json" \
  --hierarchical \
  --name "$project_name" \
  --version "$product_version"

enrich_global_sbom "$output_path/bom-all-products-raw.json" "$output_path/bom-all-products.json"
rm "$output_path/bom-all-products-raw.json"

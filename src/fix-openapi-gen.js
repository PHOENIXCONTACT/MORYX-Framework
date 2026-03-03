#!/usr/bin/env node

/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/**
 * Post-generation script for ng-openapi-gen
 * Deletes generated Entry-related files and updates imports to use @moryx/ngx-web-framework
 *
 * Additionally it adds the copyright header to all generated files.
 */

const fs = require('fs');
const path = require('path');

// Read output directory from openapi-gen.json in current working directory
const cwd = process.cwd();
const configPath = path.join(cwd, 'openapi-gen.json');
const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));
const apiDir = path.join(cwd, config.output || 'src/app/api');
const apiModulesDir = path.join(apiDir, 'models');

const FRAMEWORK_IMPORT = '@moryx/ngx-web-framework/entry-editor';

// Files to delete and their corresponding type names
const entryTypes = {
  'entry': 'Entry',
  'entry-value': 'EntryValue',
  'entry-validation': 'EntryValidation',
  'entry-unit-type': 'EntryUnitType',
  'entry-value-type': 'EntryValueType',
  'entry-possible': 'EntryPossible',
  'data-type': 'DataType',
};

// Generate import patterns dynamically from entryTypes
function buildImportPattern(moduleName) {
  // Matches: from './entry', from '../models/entry', from '../../models/entry', etc.
  const escaped = moduleName.replace(/-/g, '\\-');
  return new RegExp(`from\\s+['"](?:\\.\\.\\/)*(?:models\\/)?${escaped}['"];?`);
}

function buildExportPattern(moduleName) {
  // Matches: export { X } from './models/entry'; or export type { X } from './models/entry';
  const escaped = moduleName.replace(/-/g, '\\-');
  return new RegExp(`export\\s*(type\\s*)?\\{[^}]*\\}\\s*from\\s*['\"]\\.\\/models\\/${escaped}['\"];?\\n?`, 'g');
}

const COPYRIGHT_HEADER = `/*
 * Copyright (c) ${new Date().getFullYear()} Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

`;

console.log('Fixing Entry imports...');

// Step 1: Delete Entry-related files
console.log('\nDeleting Entry files:');
for (const moduleName of Object.keys(entryTypes)) {
  const filePath = path.join(apiModulesDir, `${moduleName}.ts`);
  if (fs.existsSync(filePath)) {
    fs.unlinkSync(filePath);
    console.log(`  Deleted: ${moduleName}.ts`);
  }
}

// Step 2: Update imports in all .ts files
console.log('\nUpdating imports in generated files:');

function processDirectory(dir) {
  if (!fs.existsSync(dir)) return;

  const files = fs.readdirSync(dir);
  for (const file of files) {
    const filePath = path.join(dir, file);
    const stat = fs.statSync(filePath);

    if (stat.isDirectory()) {
      processDirectory(filePath);
    } else if (file.endsWith('.ts')) {
      let content = fs.readFileSync(filePath, 'utf8');
      let modified = false;
      const typesToImport = new Set();

      // Add copyright header if missing
      if (!content.startsWith('/*\n * Copyright')) {
        content = COPYRIGHT_HEADER + content;
        modified = true;
      }

      // Find and collect all Entry types that need to be imported
      for (const [moduleName, typeName] of Object.entries(entryTypes)) {
        const fromPattern = buildImportPattern(moduleName);
        const importPattern = new RegExp(`import\\s*\\{[^}]+\\}\\s*${fromPattern.source}`, 'g');
        if (importPattern.test(content)) {
          typesToImport.add(typeName);
          // Remove the old import line
          content = content.replace(new RegExp(`${importPattern.source}\\n?`, 'g'), '');
          modified = true;
        }
      }

      // Add consolidated import from framework if needed
      if (typesToImport.size > 0) {
        const types = Array.from(typesToImport).sort().join(', ');
        const newImport = `import { ${types} } from '${FRAMEWORK_IMPORT}';\n`;

        // Add import after the eslint-disable comment
        if (content.includes('/* eslint-disable */')) {
          content = content.replace('/* eslint-disable */', `/* eslint-disable */\n${newImport}`);
        } else {
          content = newImport + content;
        }
      }

      if (modified) {
        fs.writeFileSync(filePath, content);
        console.log(`  Updated: ${path.relative(apiDir, filePath)}`);
      }
    }
  }
}

processDirectory(apiDir);

// Step 3: Update models.ts barrel file to remove Entry exports
const modelsBarrel = path.join(apiDir, 'models.ts');
if (fs.existsSync(modelsBarrel)) {
  let content = fs.readFileSync(modelsBarrel, 'utf8');
  let modified = false;

  for (const moduleName of Object.keys(entryTypes)) {
    const exportPattern = buildExportPattern(moduleName);
    if (exportPattern.test(content)) {
      exportPattern.lastIndex = 0;
      content = content.replace(exportPattern, '');
      modified = true;
    }
  }

  if (modified) {
    fs.writeFileSync(modelsBarrel, content);
    console.log(`  Updated: models.ts`);
  }
}

console.log('\nDone.');

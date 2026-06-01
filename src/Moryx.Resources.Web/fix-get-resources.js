const fs = require('fs');

const file = 'src/app/api/fn/resource-modification/get-resources.ts';

if (!fs.existsSync(file)){
    console.log(`fix-get-resources: ${file} not found.`);
} else {
    const oldLine = "rb.query('IncludedReferences', params.IncludedReferences, {});";

    const newLines =
        "params.IncludedReferences?.forEach((ref, index) => {\n" +
        "      if (ref.name !== undefined) rb.query(`IncludedReferences[${index}].name`, ref.name, {});\n" +
        "      if (ref.relationType !== undefined) rb.query(`IncludedReferences[${index}].relationType`, ref.relationType, {});\n" +
        "      if (ref.role !== undefined) rb.query(`IncludedReferences[${index}].role`, ref.role, {});\n" +
        "      if (ref.valueConstraint !== undefined) rb.query(`IncludedReferences[${index}].valueConstraint`, ref.valueConstraint, {})\n" +
        "    })";

    const content = fs.readFileSync(file, 'utf8');
    if (content.includes(oldLine)){
        fs.writeFileSync(file, content.replace(oldLine, newLines));
        console.log('fix-get-resources: Adjusted IncludedReferences in get-resources.ts');
    } else{
        console.log('fix-get-resources: Line not found, nothing changed.')
    }
}
/**
 * Shared MORYX ESLint configuration for Angular projects.
 * Import and spread in each project's eslint.config.mjs.
 *
 * Usage:
 *   import tseslint from "typescript-eslint";
 *   import { moryxConfig } from "../../eslint.config.mjs";
 *   export default tseslint.config(...moryxConfig(tseslint, angular));
 */
export function moryxConfig(tseslint, angular) {
  return [
    {
      ignores: ["**/api/**"],
    },
    {
      files: ["**/*.ts"],
      extends: [
        ...tseslint.configs.recommended,
        ...angular.configs.tsRecommended,
      ],
      processor: angular.processInlineTemplates,
      rules: {
        // Angular style guide: use lifecycle hook interfaces
        "@angular-eslint/use-lifecycle-interface": "error",

        // Allow ChangeDetectionStrategy.Eager (used across all MORYX components)
        "@angular-eslint/prefer-on-push-component-change-detection": "off",

        // Angular style guide: component selector prefix
        "@angular-eslint/component-selector": [
          "error",
          {
            type: "element",
            prefix: "app",
            style: "kebab-case",
          },
        ],

        // Angular style guide: directive selector prefix
        "@angular-eslint/directive-selector": [
          "error",
          {
            type: "attribute",
            prefix: "app",
            style: "camelCase",
          },
        ],

        // Always require braces around if/for/while/else blocks
        "curly": "error",

        // Relax some typescript-eslint rules for Angular patterns
        "@typescript-eslint/no-empty-function": "off",
        "@typescript-eslint/no-explicit-any": "warn",

        // Only flag unused variables, not unused function arguments
        "@typescript-eslint/no-unused-vars": [
          "error",
          { args: "none", varsIgnorePattern: "^_", caughtErrors: "none" },
        ],
      },
    },
    {
      files: ["**/*.html"],
      extends: [
        ...angular.configs.templateRecommended,
        ...angular.configs.templateAccessibility,
      ],
      rules: {
        // Angular style guide: prefer native class/style bindings
        "@angular-eslint/template/no-negated-async": "error",

        // Disabled: Angular Material components (mat-tab-link, mat-icon-button, etc.)
        // already handle keyboard accessibility internally, causing false positives.
        "@angular-eslint/template/click-events-have-key-events": "off",
        "@angular-eslint/template/interactive-supports-focus": "off",
      },
    },
  ];
}

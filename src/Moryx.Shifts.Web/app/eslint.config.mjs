import tseslint from "typescript-eslint";
import angular from "angular-eslint";
import { moryxConfig } from "@moryx/eslint-config";

export default tseslint.config(...moryxConfig(tseslint, angular));

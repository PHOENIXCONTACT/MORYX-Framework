import tseslint from "typescript-eslint";
import angular from "angular-eslint";
import { moryxConfig } from "../../eslint.config.mjs";

export default tseslint.config(...moryxConfig(tseslint, angular));

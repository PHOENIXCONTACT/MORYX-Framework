---
uid: Decisions
---
# Decisions & why we made them

There are endless possibilities to create a web based JavaScript UI that is difficult to differentiate the best approach to build your application. But some decisions are easy to made, so Node.js is set because there is no other alternative in sight. Node.js comes with npm a package manager that is very common. But stop what about other package managers?

## Build &amp; Tools

This section describes the tools which are needed to build and deploy the application.

### npm vs. yarn

Yarn is a good alternative to npm because it has a global cache on developers host that makes it faster to get all needed packages. And yes, first we used yarn but we had problems with some bugs and had problems to get all packages to computers behind companies proxy. So we made a U-turn and now using npm.

Maybe yarn will be a choice in future if it solves all problems we had with.

### TypeScript

As you might know JavaScript is a weakly typed language like PHP or Python. And what seems to be easier to implement at the first glance will be a bad trip if your application gets bigger and bigger.

So Microsoft invented TypeScript which gives you the possibility to create strong typed JavaScript programs. That sounds great and eliminates many hazzles with wrong typo, not compatible types or unreadable code.

So that's the theory. In fact there is a little problem if you are using TypeScript: You need the types for third party libraries. Currently there are many additional type libraries (most of them starting with `@types/`) but may be not for all. And this could be a problem because you cannot use those libraries. But there is a way to solve this little problem: you need to write your own type definitions for the third party library. That's the price but it's worth.

And now comes the difficult part. TypeScript comes with it's own "compiler" which is more like a transpiler. It transpiles the TypeScript code to standard `es6` (`6th edition - ECMAScript 2015`) javascript code which is executable by the most modern browsers. An that's the trick of TypeScript: it's is used at compile, ahh, transpile time and not at runtime.

#### Lint it, baby

A `linter` is like `ReSharper` just for JavaScript (e.g. `ESLint`) or Typescript (e.g. `TSLint`). It checks for styling, suspicious constructs and so on. We are using `TSLint` to establish a kind of standard due using the standard settings of `TSLint`.

We are also switched on the automatic fix feature that is available for some settings. That fixes the source code while transpiling.

### Webpack

You can compare `Webpack` to classical compiler & linker. We use it to bundle our package so that you will find only one so called `bundel.js` file. `Webpack` is configured to use `TypeScript` as our source language, it takes care of bundling all images and scss files and it creates out `Source map` so that we are able to debug our application.

## Code base

The previous section describes what tools are needed to build the application. This section explains which JavScript libraries are used and why we had choosen these frameworks.

### React

`HTML5` is powerful but it lacks from some useful features like automatic partial DOM rerendering. In early stage of development we decided to use one of the available JavaScript UI libraries:

- React (Facebook)
- AngularJS (Google)
- Vue.js (Community)

Vue.js is too unknown and AngularJS had to many breaking API changes in the past so that significant and expansive UI reworks would be neccessary. Our choice comes to `React` built by Facebook. It is lightweight and easy to understand. It has all needed features for partial rerendering and a quite easy setup.

### Reactstrap

A mentioned above `React` is lightweight and we don't want to reinvent the wheel by creating all neccessary standard control once more. Also we want to take benifit of the new responsive feature that comes with HTML5.

Unfortunately the standard `Bootstrap` library should not be used with `React` together but we were lucky that someone created a Bootstrap React variant called `reactstrap`. It comes with the most important controls und does fully support `React`.

### Redux

Last but not least we were missing a data layer which holds all of our data and made it available to all components. After some reading and testing we've found the `Redux` library that is our data layer tool.

## Conclusion

In fact we used very common toolset and libraries to create our application. However the first steps were hard because toolset and libraries are common but may be not in the combination with `TypeScript`.

If the build chain is setup successfully the application grows fast.
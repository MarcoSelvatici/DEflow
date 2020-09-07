# DEflow

DEflow is an application for digital circtuit design and simulation. It is targeted to students and hobbysts that want to get a grasp of Digital Electronics concepts in a simple and fun way. DEflow is designed to be beginner-friendly and guide the users toward their goals via clear error messages and visual clues.

The application has been developed by Marco Selvatici as a its graduation project.

The application has since then been extended and maintained by the project supervisor (Tom Clarke) and other developers. You can find the most up to date versions in [another github repo](https://github.com/tomcl/issie/). These versions should have more features and are in general recommended for teaching porpuses.

If you are just interested in using this version of the application, jump to the [Getting Started](#getting-started) section. For more info about the project, read on.

This documentation is largely based on the excellent [VisUAL2](https://github.com/ImperialCollegeLondon/Visual2) documentation, given the similarity in the technology stack used.

## Notable Resources

- [Marco's dissertation](https://github.com/MarcoSelvatici/DEflow/blob/master/dissertation.pdf),
- [Marco's poster](https://github.com/MarcoSelvatici/DEflow/blob/master/poster.pdf) (for a shorter and more visually appealing explanation of the project).

## Introduction

The application is mostly written in F#, which gets transpiled to JavaScript via the [fable](https://fable.io/) compiler. [Electron](https://www.electronjs.org/) is then used to convert the developed web-app to a cross-platform application. Electron provides access to platform-level APIs (such as access to the file system) which would not be available to vanilla browser web-apps.

[Webpack 4](https://webpack.js.org/) is the module bundler responsible for the JavaScript concatenation and automated building process.

The drawing capabilities are provided by the [draw2d](http://www.draw2d.org/draw2d/) JavaScript library, which has been extended to support digital electronics components.

The choice of F# as main programming language for the app has been dictated by a few factors:
* the success of the [VisUAL2](https://github.com/ImperialCollegeLondon/Visual2), which uses a similar technology stack;
* strongly typed functional code tends to be easy to maintain and test, as the type-checker massively helps you;
* Imperial College EEE/EIE students learn such language in the 3rd year High-Level-Programming course, hence can maintain the app in the future;
* F# can be used with the powerful [Elmish](https://elmish.github.io/elmish/) framework to develop User Interfaces in a [Functional Reactive Programming](https://en.wikipedia.org/wiki/Functional_reactive_programming) fashion.

## Project Structure

Electron bundles Chromium (View) and node.js (Engine), therefore as in every node.js project, the `package.json` file specifies the (Node) module dependencies.

* dependencies: node libraries that the executable code (and development code) needs
* dev-dependencies: node libraries only needed by development tools

Additionally, the section `"scripts"`:
```
{
    ...
    "scripts": {
        "start": "cd src/Main && dotnet fable webpack --port free -- -w --config webpack.config.js",
        "build": "cd src/Main && dotnet fable webpack --port free -- -p --config webpack.config.js",
        "launch": "electron .",
        "debug": "electron . --debug",
    },
    ...
}
```
Defines the in-project shortcut commands, therefore when we use `yarn <stript_key>` is equivalent to calling `<script_value>`. For example, in the root of the project, running in the terminal `yarn launch` is equivalent to running `electron .`.

## Code Structure

The source code consists of two distinct sections transpiled separately to Javascript to make a complete Electron application.

* The electron main process runs the Electron parent process under the desktop native OS, it starts the app process and provides desktop access services to it.
* The electron client (app) process runs under Chromium in a simulated browser environment (isolated from the native OS).

Electron thus allows code written for a browser (HTML + CSS + JavaScript) to be run as a desktop app with the additional capability of desktop filesystem access via communication between the two processes.

Both processes run Javascript under Node.

The `src/Main/Main.fs` source configures electron start-up and is boilerplate. It is transpiled to the root project directory so it can be automatically picked up by Electron.

The remaining app code is arranged in five different sections, each being a separate F# project. This separation allows all the non-web-based code (which can equally be run and tested under .Net) to be run and tested under F# directly in addition to being transpiled and run under Electron.

The project relies on the draw2d JavaScript library, which is extended to support digital electronics components. The extensions are in the `app/public/lib/draw2d_extensions` folder and are loaded by the `index.html` file. The `index.html` file is otherwise empty as the UI elements are dynamically generated with [React](https://reactjs.org/), thanks to the F# Elmish library.

The code that turns the F# project source into `renderer.js` is the FABLE compiler followed by the Node Webpack bundler that combines multiple Javascript files into a single `renderer.js`. Note that the FABLE compiler is distributed as a node package so gets set up automatically with other Node components.

The compile process is controlled by the `.fsproj` files (defining the F# source) and `webpack.config.js` which defines how Webpack combines F# outputs for both electron main and electron app processes and where the executable code is put. This is boilerplate which you do not need to change; normally the F# project files are all that needs to be modified.

## File Structure

### `src` folder

|   Subfolder   |                                             Description                                            |
|:------------:|:--------------------------------------------------------------------------------------------------:|
| `Common/`       | Provides some common types and utilities used by all other sections                                |
| `WidthInferer/` | Contains the logic to infer the width of all connections in a diagram and report possible errors. |
| `Simulator/`    | Contains the logic to analyse and simulate a diagram.                                              |
| `Renderer/`     | Contains the UI logic, the wrapper to the JavaScript drawing library and a set of utility function to write/read/parse diagram files. This is the only project that cannot run under .Net, as it contains JavaScript related functionalities. |
| `Tests/`        | Contains numerous tests for the WidthInferer and Simulator. Based on F# Expecto testing library. |


### `app` folder

| Subfolder or file |                                                                                                                                                          Description                                                                                                                                                          |
|:-----------------:|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| `public/lib/`     | Contains JavaScript code loaded by the `index.html` file. It includes the draw2d library itself, its custom extensions and jquery. Note that draw2d and jquery may be copied (and maybe should?) from the node modules when using the `copy-webpack-plugin`. See the comment in `webpack.config.js` for more info about this. |
| `scss/`           | Stylesheet required by the [Fulma](https://fulma.github.io/Fulma/) UI library (F# port of [bulma](https://bulma.io/)).                                                                                                                                                                                                        |
| `icon.ico`        | DEflow icon.                                                                                                                                                                                                                                                                                                                  |
| `index.html`      | The page rendered by the renderer process. The HTML DOM is dynamically generated using React when the app is running.                                                                                                                                                                                                         |
| Other             | Other files will be generated in this folder at compilation time. They are ignored by the gitignore, and you don't have to worry about those.                                                                                                                                                                                 |

## Concept of Project and File in DEflow

DEflow allows the users to create projects and files within those projects. A DEflow project is simply a folder named `<project_name>.dprj` (dprj stands for diagram project). A project contains a collection of designs, each named `<component_name>.dgm` (dgm stands for diagram).

When opening a project, DEflow will search the given repository for `.dgm` files, parse their content, and allow the user to open them in DEflow or use them as components in other designs.

## Getting Started

If you just want to run the app go to the [releases page](https://github.com/ms8817/FIY/releases) and follow the instructions on how to download and run the prebuilt binaries.

If you want to get started as a developer, follow these steps:

1. Follow instructions to install [yarn](https://yarnpkg.com/lang/en/docs/install/) (which tell you to install Node as well).

2. Download and install the latest (2.x) [Dotnet Core SDK](https://www.microsoft.com/net/learn/get-started).  
For Mac and Linux users, download and install [Mono](http://www.mono-project.com/download/stable/) from official website (the version from brew is incomplete, may lead to MSB error on step 6).

3. Download & unzip the DEflow repo, or if contributing clone it locally, or fork it on github and then clone it locally.

4. Navigate to the project root directory (which contains this README) in a command-line interpreter. For Windows usage make sure if possible for convenience that you have a _tabbed_ command-line interpreter that can be started direct from file explorer within a specific directory (by right-clicking on the explorer directory view). That makes things a lot more pleasant. The new [Windows Terminal](https://github.com/microsoft/terminal) works well.

5. Fetch the required `npm` packages by executing `yarn install`. This project consistently uses `yarn` Node package manager instead of `npm`.

6. On MacOS or Linux ensure you have [paket installed](https://fsprojects.github.io/Paket/installation.html). Run `setup.bat` (on Windows) or `sh setup.sh` (on linux or macOS). This installs all the required nuget and node dependencies. On other systems run the statements in this file (modified if needed for your system) individually. If MSB error occur while running the script (on macOS) and you were using Mono installed by brew previously, run `brew uninstall mono` and refer to step 2 for install Mono correctly).

7. Goto step 10 if all you want to do is to generate uptodate binaries.

8. In a terminal window compile `fsharp` code to `javascript` using `webpack` by executing `yarn start` (shortcut for `yarn run start`). This runs the `start` script defined in `package.json`. The `start` script  compiles everything once and then watches source files recompiling whenever any change, so it is normal run continuously throughout development. You will need to view the `yarn start` output throughout development since if compile fails the output makes this clear via red-colored text. Although Ionide will also provide error messages on code that does not compile it is possible to miss these when making quick changes.

9. Open your `electron` app in a new terminal tab by running `yarn launch`. This command will start the application.

10. Run `yarn pack-win`, `yarn pack-linux`, `yarn pack-osx` at any time to create a set of system-specific self-contained binaries in `./dist/os-name/*` and a zip in `./dist`. Each binary distribution consists of a portable directory with all dependencies, so use the appropriate one of these if you just want to run DEflow. For osx, the easiest way to run DEflow once it has been built is to navigate to `./dist/DEflow-darwin-x64` and execute `open -a DEflow.app` in terminal. Note that some host-target combinations will not correctly generate: `pack-osx must be executed on os-x`.

11. To open the Chromium console from the running DEflow app press `Ctrl-Shift-I`.

## Reinstalling Compiler and Libraries

The code requires a global installation of `dotnet` and `node`/`npm`. This does not need changing and is unlikely to cause trouble. Later versions of dotnet SDK or node can usually be installed without trouble

All the dependencies are local and installed by yarn (node modules) or dotnet (dotnet assemblies). 

WARNING: `dotnet` assemblies are cached locally at machine level by dotnet. This sometimes goes wrong leading to strange compilation errors. It can be cured very simply by clearing the `dotnet` assembly caches, which is done in `setup.bat` and `setup.sh`.

To reinstall the build environment (without changing project code) run `setup.bat` (Windows) or `setup.sh` (Linux and MacOS).

## Creating DEflow Binaries

After you have compiled code (and checked it works) `yarn pack-all` will run electron packager and generate `./dist/os-name/*` files.

Useful shortcuts for specific common target OS:
* `yarn pack-win` (Windows)
* `yarn pack-linux` (Linux)
* `yarn pack-osx` (MacOs)

I could not test packaging for MacOS yet as I do not have access to a MacOS device at the moment.

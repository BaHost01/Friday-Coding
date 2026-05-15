# Friday-Coding Project Overview

Friday-Coding is a simple engine designed for creating User Interfaces (UI) and scripting mods for Friday Night Funkin' (FNF). It is built with C# and aims to provide a beginner-friendly environment with tutorials and exporting capabilities.

## Architecture and Structure

The project is organized into several key directories, each serving a specific purpose in the modding workflow:

- **CustomBytecode/**: Likely intended for storing or processing custom bytecode for the engine.
- **CustomLua/**: A dedicated space for custom Lua scripts that extend the engine's functionality.
- **Exporting/**: Contains tools or configurations for exporting completed mods.
- **Lua/**: Core Lua scripting resources and base scripts.
- **Shared/**: Shared assets, libraries, or code used across different modules.
- **SyntaxChecking/**: Logic or tools for validating Lua scripts and other configurations.
- **Templates/**: Pre-defined templates for UI and scripts to help beginners get started.
- **TestingEnvironment/**: A sandbox or environment for testing mods before they are finalized.
- **UserInterface/**: Resources and code related to the engine's UI creation capabilities.

## Technologies

- **Primary Language**: C#
- **Scripting Language**: Lua

## Development and Usage

### Getting Started
As the project is an engine for modding, development involves:
1. Creating UI components using the tools in the `UserInterface/` directory.
2. Writing Lua scripts in the `Lua/` or `CustomLua/` directories.
3. Using the `Templates/` to kickstart new mod components.

### Building and Running
- **Build System**: [TODO: Identify build system, e.g., MSBuild, dotnet CLI]
- **Run Command**: [TODO: Identify how to launch the engine]
- **Test Command**: [TODO: Identify how to run tests in the TestingEnvironment]

### Conventions
- **Scripting**: Follow Lua best practices for FNF modding.
- **UI Design**: Leverage the templates provided in the `Templates/` directory to maintain consistency.

## Future Instructions
- When adding new features, ensure they are modularized into the appropriate directory (e.g., new scripting features in `Lua/`, new UI elements in `UserInterface/`).
- Documentation should be updated in the `README.md` and referenced here in `GEMINI.md`.

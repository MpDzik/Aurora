# Aurora

Aurora is a framework which helps building complex multi-module console-based applications based on the .NET Framework.

## Features

The framework provides the following functionality.

- Dependency injection support based on [Autofac](https://github.com/autofac/Autofac) library
- Automatic command line argument parsing based on [CommandLineParser](https://github.com/commandlineparser/commandline) library
- Environment abstractions (I/O, clock, etc.) for better architecture, minimal coupling and better unit testing

## Components and Modules

The framework consits of the following components.

### Aurora.Core

The framework's core code which applications and modules are based on. You can reference this assembly from your console application to use the Aurora framework.

### Modules
Contains additional modules which can be added to a console application to add new functionality, integration with external libraries, etc.

### Aurora.Tests
Contains unit tests for the framework's core code and modules.

### Samples\Aurora.Sample
Sample console application based on the Aurora framework.

### Samples\Aurora.SampleModule
Sample Aurora framework module.

**This project is still in an early stage of development.**

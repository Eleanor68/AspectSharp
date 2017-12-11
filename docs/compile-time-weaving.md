# Compile Time Weaving

This page contains design consideration and process of the compile time weaving.

The AspectSharp.Build project will contain all tools and instrumentation for integration of AspectSharp in the build process (based on msbuild).
The AspectSharp.Targets file will contain task definitions that will perform discovery and codegen.

## MSBuild integration

1. The project should import `AspectSharp.Targets`
2. BeforeBuild
    * Perform aspect definition discovery and store them in memory (if applicable) or generate an assembly/json
3. BeforeCompile
    1. Generate stubs
    2. Re-generate @(Compile) based on generated stubs
4. The compile process continue ...

## Aspect Discovery and Database

In first instance the AspectSharp have to collect all aspect and advice definition for projects and custom referenced assemblies.
If msbuild executes tasks in one AppDomain then AspectSharp can use shared memory for definitions database.
Otherwise AspectSharp have to store the database in an optional format assembly/json and use it for code generation.

## Code Generation (codegen)
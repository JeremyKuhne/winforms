﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using System.CodeDom;
using System.Runtime.Loader;
using System.Reflection;

namespace System.Resources.Tools.Tests;

internal static class CodeDomCompileHelper
{
    private static readonly CodeDomProvider s_cSharpProvider = new CSharpCodeProvider();

    private static MetadataReference[] References { get; } = CreateReferences();

    private static MetadataReference[] CreateReferences()
    {
        string corelibPath = typeof(object).Assembly.Location;
        return new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Join(Path.GetDirectoryName(corelibPath), "System.Runtime.dll")),
        };
    }

    private static Stream CreateAssemblyImage(string source, string assemblyName, string resourceName, Stream resource)
    {
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { CSharpSyntaxTree.ParseText(source) },
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        ResourceDescription[] description = resource is null
            ? null
            : new ResourceDescription[] { new(resourceName, () => resource, isPublic: true) };

        MemoryStream stream = new();
        if (compilation.Emit(stream, manifestResources: description).Success)
        {
            stream.Position = 0;
            return stream;
        }

        throw new InvalidOperationException();
    }

    internal static Type CompileClass(
        CodeCompileUnit compileUnit,
        string baseName,
        string nameSpace = null,
        Stream resource = null)
    {
        AssemblyLoadContext context = new(name: null, isCollectible: true);
        string fullName = nameSpace is null ? baseName : $"{nameSpace}.{baseName}";

        using StringWriter writer = new();
        s_cSharpProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new());
        Assembly assembly = context.LoadFromStream(CreateAssemblyImage(
            writer.ToString(),
            fullName,
            $"{fullName}.resources",
            resource));
        Type type = assembly.GetType(fullName, throwOnError: true);

        // Once all references are collected, the assembly will unload.
        context.Unload();
        return type;
    }
}

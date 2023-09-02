using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CSharpElevatorSaga.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace CSharpElevatorSaga.Compilation;

public class Compiler
{
    private const string UsingsFilePath = "UsingsFile";
    private readonly IAssemblyResolver _assemblyResolver;
    private int _assemblyNumber;
    private bool _warmedUp;
    private static readonly IReadOnlyList<string> AssemblyFileNames = new[]
        {
            "System.Collections.dll",
            "System.Console.dll",
            "System.Linq.dll",
            "System.Private.CoreLib.dll",
            "System.Runtime.dll",
            typeof(IElevator).Assembly.ManifestModule.Name
        };
    private IReadOnlyList<MetadataReference>? _assemblyMetadata;
    public Compiler(IAssemblyResolver assemblyResolver)
    {
        _assemblyResolver = assemblyResolver;
    }

    [MemberNotNull(nameof(_assemblyMetadata))]
    public async Task PreloadAssemblies()
    {
        if (_warmedUp)
        {
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
            return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
        }

        _warmedUp = true;
        var metadata = new List<MetadataReference>();
        foreach (var assemblyFileName in AssemblyFileNames)
        {
            try
            {

#pragma warning disable CS8774 // Member must have a non-null value when exiting.
            MetadataReference reference = await _assemblyResolver.GetAssembly(assemblyFileName);
#pragma warning restore CS8774 // Member must have a non-null value when exiting.
            metadata.Add(reference);
            }
            catch
            {
                Console.WriteLine("Failed to load " + assemblyFileName);
            }
        }


        _assemblyMetadata = metadata;
    }

    public async Task<CompileResult> CompileCodeAsync(string code)
    {
        if(!_warmedUp)
        {
            await PreloadAssemblies();
        }

        var usingStringSyntaxTree = MakeUsingSyntaxTree();

        List<Diagnostic> diagnostics = new();

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Latest), "UserCode");
        foreach (var diagnostic in syntaxTree.GetDiagnostics())
        {
            AddDiagnostic(diagnostic, diagnostics);
        }
        var diagnosticsConfiguration = new List<KeyValuePair<string, ReportDiagnostic>>()
        {
            new("CS8321", ReportDiagnostic.Hidden)
        };
        CSharpCompilation compilation = CSharpCompilation.Create(
            "Solution" + _assemblyNumber++,
            new[] { syntaxTree, usingStringSyntaxTree },
            _assemblyMetadata,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication, false, specificDiagnosticOptions: diagnosticsConfiguration, concurrentBuild: false));

        using (MemoryStream stream = new())
        {
            EmitResult result = compilation.Emit(stream);

            foreach (var diagnostic in result.Diagnostics)
            {
                AddDiagnostic(diagnostic, diagnostics);
            }

            if (!result.Success)
            {
                return CompileResult.Fail(diagnostics);
            }

            return LoadAssembly(stream, diagnostics);
        }
    }

    private static void AddDiagnostic(Diagnostic diagnostic, List<Diagnostic> diagnostics)
    {
        if (diagnostic.Severity == DiagnosticSeverity.Hidden)
        {
            return;
        }

        if (diagnostic.Id == "CS5001")
        {
            diagnostic = Diagnostic.Create(NoRunMethodRule, Location.None);
        }

        diagnostics.Add(diagnostic);
    }

    private static SyntaxTree MakeUsingSyntaxTree()
    {
        var usings = new[] {
            "System",
            "System.Linq",
            "System.Collections",
            "System.Collections.Generic",
            typeof(IElevator).Namespace!
        };
        string usingStringSyntax = "#pragma warning disable\n" + string.Join("\n", usings.Select(n => "global using " + n + ";"));
        return CSharpSyntaxTree.ParseText(usingStringSyntax, new CSharpParseOptions(LanguageVersion.Latest), UsingsFilePath);
    }

    private static CompileResult LoadAssembly(MemoryStream stream, List<Diagnostic> diagnostics)
    {
        Assembly assembly = AppDomain.CurrentDomain.Load(stream.ToArray());
        var type = assembly.GetTypes().FirstOrDefault(n => n.Name == "Program");

        if (type == null)
        {
            diagnostics.Add(Diagnostic.Create(NoRunMethodRule, Location.None));
            return CompileResult.Fail(diagnostics);
        }

        var methodInfo = type.GetMethod("<<Main>$>g__Run|0_0", BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
        if (methodInfo == null || !methodInfo.IsStatic)
        {
            diagnostics.Add(Diagnostic.Create(NoRunMethodRule, Location.None));
            return CompileResult.Fail(diagnostics);
        }

        var programDelegate = (ProgramEntryPoint.RunProgram?)Delegate.CreateDelegate(typeof(ProgramEntryPoint.RunProgram), methodInfo, false);
        if (programDelegate == null)
        {
            diagnostics.Add(Diagnostic.Create(RunMethodHasInvalidSignatureRule, Location.None));
            return CompileResult.Fail(diagnostics);
        }

        return CompileResult.Compiled(diagnostics, programDelegate);
    }

    internal static DiagnosticDescriptor NoRunMethodRule = new("ES0001", "Could not find Run", "Program does not contain static void Run(IElevator[] elevators, IFloor[] floors) function",
  "Elevator Saga", DiagnosticSeverity.Error, isEnabledByDefault: true);

    internal static DiagnosticDescriptor RunMethodHasInvalidSignatureRule = new("ES0002", "Invalid Run method", "Function Run should have signature static void Run(IElevator[] elevators, IFloor[] floors)",
  "Elevator Saga", DiagnosticSeverity.Error, isEnabledByDefault: true);

    public class CompileResult
    {
        public CompileResult(bool success, IReadOnlyList<Diagnostic> diagnostics, ProgramEntryPoint.RunProgram? program)
        {
            Success = success;
            Diagnostics = diagnostics;
            Program = program;
        }

        public static CompileResult Fail(IReadOnlyList<Diagnostic> diagnostics) => new(false, diagnostics, null);

        public static CompileResult Compiled(IReadOnlyList<Diagnostic> diagnostics, ProgramEntryPoint.RunProgram program) => new(true, diagnostics, program);

        [MemberNotNullWhen(true, nameof(Program))]
        public bool Success { get; }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }

        public ProgramEntryPoint.RunProgram? Program { get; }

    }
}
using CSharpElevatorSaga.Compilation;

namespace CSharpElevatorSaga.UnitTests.CompilerTests;

public class CompilerDiagnosticTests
{
    [Theory]
    [InlineData("void Run(IElevator[] elevators, IFloor[] floors) {}")]
    [InlineData("static void Run(IElevator[] elevators, IFloor[] floors) {}")]
    public async Task Should_MinimalValidProgram_CompileSuccessfully(string program)
    {
        var result = await new Compiler(new AssemblyResolver()).CompileCodeAsync(program);

        Assert.True(result.Success);
        Assert.Empty(result.Diagnostics);
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData("Console.WriteLine(\"Whatever\");")]
    [InlineData("static string LancelotsFavoriteColor() { return \"Blue\"; }")]
    public async Task Should_EmptyProgram_Emit_NoRunMethodDiagnostic(string program)
    {
        var result = await new Compiler(new AssemblyResolver()).CompileCodeAsync(program);

        Assert.False(result.Success);
        Assert.Single(result.Diagnostics);
        var diagnostics = result.Diagnostics.Single();
        Assert.Equal("ES0001", diagnostics.Descriptor.Id);
    }

    [Theory]
    [InlineData("int x = 5;")]
    public async Task Should_SimpleInvalidPrograms_Emit_NoRunMethodDiagnostic(string program)
    {
        var result = await new Compiler(new AssemblyResolver()).CompileCodeAsync(program);

        Assert.False(result.Success);
        Assert.Contains(result.Diagnostics, n => n.Id == "ES0001");
    }

    [Theory]
    [InlineData("int Run() { return 5; }")]
    [InlineData("void Run() {}")]
    [InlineData("void Run(IFloor[] floors) {}")]
    [InlineData("void Run(IElevator[] elevators) {}")]
    [InlineData("void Run(IFloor[] elevators, IFloor[] floors) {}")]
    [InlineData("void Run(IElevator[] elevators, IFloor[] floors, int stuff) {}")]
    public async Task Should_InvalidRunSignature_Emit_InvalidSignatureDiagnostic(string program)
    {
        var result = await new Compiler(new AssemblyResolver()).CompileCodeAsync(program);

        Assert.False(result.Success);
        Assert.Contains(result.Diagnostics, n => n.Id == "ES0002");
    }

}

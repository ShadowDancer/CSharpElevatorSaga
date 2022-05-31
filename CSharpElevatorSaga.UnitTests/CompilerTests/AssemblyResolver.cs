using CSharpElevatorSaga.Compilation;
using Microsoft.CodeAnalysis;

namespace CSharpElevatorSaga.UnitTests.CompilerTests;

internal class AssemblyResolver : IAssemblyResolver
{
    public Task<MetadataReference> GetAssembly(string name)
    {
        try
        {

            var assembly = AppDomain.CurrentDomain.GetAssemblies().Single(n => n.ManifestModule.Name == name);
            MetadataReference assemblyReference = MetadataReference.CreateFromFile(assembly.Location);
            return Task.FromResult(assemblyReference);
        }
        catch
        {
            throw new InvalidOperationException("Exception while loadding assembly " + name);
        }
    }
}

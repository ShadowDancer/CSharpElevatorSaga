using Microsoft.CodeAnalysis;

namespace CSharpElevatorSaga.Compilation;

public interface IAssemblyResolver
{
    public Task<MetadataReference> GetAssembly(string assemblyName);
}
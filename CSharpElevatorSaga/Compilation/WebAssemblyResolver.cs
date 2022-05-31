using Microsoft.CodeAnalysis;

namespace CSharpElevatorSaga.Compilation;

public class WebAssemblyResolver : IAssemblyResolver
{

    private static readonly HttpClient Client = new();

    public static void SetBaseUri(string baseUri)
    {
        Client.BaseAddress = new Uri(baseUri);
    }

    public async Task<MetadataReference> GetAssembly(string assemblyName)
    {
        string fileUri = $"_framework/{assemblyName}";

        var stream = await Client.GetStreamAsync(fileUri);
        return MetadataReference.CreateFromStream(stream);
    }
}
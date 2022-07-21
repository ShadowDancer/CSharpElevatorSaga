using CSharpElevatorSaga;
using CSharpElevatorSaga.Compilation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<Compiler>();

builder.Services.AddSingleton<IAssemblyResolver, WebAssemblyResolver>();

var host = builder.Build();

WebAssemblyResolver.SetBaseUri(host.Services.GetRequiredService<NavigationManager>().BaseUri);


var run = host.RunAsync();

_ = host.Services.GetRequiredService<Compiler>().PreloadAssemblies();

await run;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using TrmrkMkMdFile;

var services = new ServiceCollection();
services.AddScoped<ProgramConfigRetriever>();
services.AddScoped<ProgramArgsRetriever>();
services.AddScoped<ProgramComponent>();

var svcProv = services.BuildServiceProvider();

UtilsH.ExecuteProgram(() =>
{
    var pgComponent = svcProv.GetRequiredService<ProgramComponent>();
    pgComponent.Run(args);
});
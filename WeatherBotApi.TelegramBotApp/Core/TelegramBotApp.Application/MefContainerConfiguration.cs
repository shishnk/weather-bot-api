using System.Composition;
using System.Composition.Hosting;

namespace TelegramBotApp.Application;

public static class MefContainerConfiguration
{
    public static void SatisfyImports<T>(object importInfo)
    {
        var assemblies = new[] { typeof(T).Assembly };
        var configuration = new ContainerConfiguration().WithAssemblies(assemblies);

        using var container = configuration.CreateContainer();
        container.SatisfyImports(importInfo);
    }
}
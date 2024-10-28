using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.Git
{
    [ZoneDefinition]
    public interface IGitZone : IZone, IRequire<ILanguageCSharpZone>
    {
    }
}

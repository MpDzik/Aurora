using System.Collections.Generic;
using System.Reflection;

namespace Aurora.Core.Infrastructure
{
    public class VersionInfoProvider : IStartupInfoProvider
    {
        public int Priority => 0;

        public IEnumerable<string> Provide()
        {
            var name = Assembly.GetEntryAssembly()?.GetName();
            var version = name?.Version;
            var build = version?.Build > 0 ? version.Build : version?.Revision;

            if (name != null && version != null)
            {
                yield return $"{name.Name} {version.Major}.{version.Minor} build {build}";
            }
        }
    }
}
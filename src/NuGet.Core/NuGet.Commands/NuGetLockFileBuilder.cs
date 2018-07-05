using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;
using NuGet.Shared;

namespace NuGet.Commands
{
    public class NuGetLockFileBuilder
    {
        private readonly int _lockFileVersion;
        public NuGetLockFileBuilder(int lockFileVersion)
        {
            _lockFileVersion = lockFileVersion;
        }

        public NuGetLockFile CreateNuGetLockFile(LockFile assetsFile)
        {
            var lockFile = new NuGetLockFile()
            {
                Version = _lockFileVersion
            };

            var libraryLookup = assetsFile.Libraries.Where(e => e.Type == LibraryType.Package)
                .ToDictionary(e => new PackageIdentity(e.Name, e.Version));

            var orderedTargets = assetsFile.Targets.OrderBy(e => e.TargetFramework)
                .ThenBy(e => e.RuntimeIdentifier, StringComparer.Ordinal);

            foreach (var target in orderedTargets)
            {
                var nuGettarget = new NuGetLockFileTarget()
                {
                    TargetFramework = target.TargetFramework,
                    RuntimeIdentifier = target.RuntimeIdentifier
                };

                var framework = assetsFile.PackageSpec.TargetFrameworks.FirstOrDefault(
                    f => EqualityUtility.EqualsWithNullCheck(f.FrameworkName, target.TargetFramework));

                foreach (var library in target.Libraries.Where(e => e.Type == LibraryType.Package))
                {
                    var identity = new PackageIdentity(library.Name, library.Version);

                    var dependency = new LockFileDependency()
                    {
                        Id = identity.Id,
                        ResolvedVersion = identity.Version,
                        Sha512 = libraryLookup[identity].Sha512,
                        Type = framework.Dependencies.Any(
                            package => PathUtility.GetStringComparerBasedOnOS().Equals(package.Name, identity.Id))
                            ? PackageInstallationType.Direct : PackageInstallationType.Transitive
                    };

                    nuGettarget.Dependencies.Add(dependency);
                }

                nuGettarget.Dependencies = nuGettarget.Dependencies.OrderBy(d => d.Type).ToList();

                lockFile.Targets.Add(nuGettarget);
            }

            return lockFile;
        }

    }
}

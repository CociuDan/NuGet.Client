using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NuGet.ProjectModel
{
    public static class NuGetLockFileUtilities
    {
        public static bool IsNuGetLockFileSupported(PackageSpec project)
        {
            return project.RestoreMetadata.RestorePackagesWithLockFile &&
                File.Exists(project.RestoreMetadata.NuGetLockFilePath);
        }

        public static string GetNuGetLockFilePath(PackageSpec project)
        {
            var path = project.RestoreMetadata.NuGetLockFilePath;

            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(project.BaseDirectory, NuGetLockFileFormat.LockFileName);
            }

            return path;
        }
    }
}

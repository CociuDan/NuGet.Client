// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NuGet.ProjectModel
{
    public class NuGetLockFileFormat
    {
        public static readonly int Version = 1;

        public static readonly string LockFileName = "nuget.lock.json";

        private const string VersionProperty = "version";
        private const string Sha512Property = "sha512";
        private const string DependenciesProperty = "dependencies";
        private const string TypeProperty = "type";

        public void Write(string filePath, NuGetLockFile lockFile)
        {
            // Create the directory if it does not exist
            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory.Create();

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Write(stream, lockFile);
            }
        }

        public void Write(Stream stream, NuGetLockFile lockFile)
        {
            using (var textWriter = new StreamWriter(stream))
            {
                Write(textWriter, lockFile);
            }
        }

        public void Write(TextWriter textWriter, NuGetLockFile lockFile)
        {
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;

                var json = WriteLockFile(lockFile);
                json.WriteTo(jsonWriter);
            }
        }

        private static JObject WriteLockFile(NuGetLockFile lockFile)
        {
            var json = new JObject
            {
                [VersionProperty] = new JValue(lockFile.Version),
                [DependenciesProperty] = WriteObject(lockFile.Targets, WriteTarget),
            };

            return json;
        }

        private static JProperty WriteTarget(NuGetLockFileTarget target)
        {
            var json = WriteObject(target.Dependencies, WriteTargetDependency);

            var key = target.Name;

            return new JProperty(key, json);
        }

        private static JProperty WriteTargetDependency(LockFileDependency dependency)
        {
            var json = new JObject();

            json[TypeProperty] = dependency.Type.ToString();

            if (dependency.Version != null)
            {
                json[VersionProperty] = dependency.Version.ToNormalizedString();
            }

            if (dependency.Sha512 != null)
            {
                json[Sha512Property] = dependency.Sha512;
            }

            return new JProperty(dependency.Id, json);
        }

        private static JObject WriteObject<TItem>(IEnumerable<TItem> items, Func<TItem, JProperty> writeItem)
        {
            var array = new JObject();
            foreach (var item in items)
            {
                array.Add(writeItem(item));
            }
            return array;
        }
    }
}

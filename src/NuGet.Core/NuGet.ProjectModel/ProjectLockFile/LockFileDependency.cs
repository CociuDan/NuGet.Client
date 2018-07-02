// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Common;
using NuGet.Shared;
using NuGet.Versioning;

namespace NuGet.ProjectModel
{
    public class LockFileDependency : IEquatable<LockFileDependency>
    {
        public string Id { get; set; }

        public NuGetVersion Version { get; set; }

        public string Sha512 { get; set; }

        public PackageInstallationType Type { get; set; }

        public bool Equals(LockFileDependency other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return PathUtility.GetStringComparerBasedOnOS().Equals(Id, other.Id) &&
                EqualityUtility.EqualsWithNullCheck(Version, other.Version) &&
                Sha512 == other.Sha512 &&
                Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LockFileDependency);
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();

            combiner.AddObject(Id);
            combiner.AddObject(Version);
            combiner.AddObject(Sha512);
            combiner.AddObject(Type);

            return combiner.CombinedHash;
        }
    }
}

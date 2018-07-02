// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using NuGet.Frameworks;
using NuGet.Shared;

namespace NuGet.ProjectModel
{
    /// <summary>
    /// FrameworkName/RuntimeIdentifier combination
    /// </summary>
    public class NuGetLockFileTarget : IEquatable<NuGetLockFileTarget>
    {
        /// <summary>
        /// Target framework.
        /// </summary>
        public NuGetFramework TargetFramework { get; }

        /// <summary>
        /// Null for RIDless graphs.
        /// </summary>
        public string RuntimeIdentifier { get; }

        /// <summary>
        /// 
        /// </summary>
        public IList<LockFileDependency> Dependencies { get; } = new List<LockFileDependency>();

        /// <summary>
        /// Full framework name.
        /// </summary>
        public string Name => GetNameString(TargetFramework.DotNetFrameworkName, RuntimeIdentifier);

        public NuGetLockFileTarget(NuGetFramework targetFramework)
            : this(targetFramework, runtimeIdentifier: null)
        {
        }

        public NuGetLockFileTarget(NuGetFramework targetFramework, string runtimeIdentifier)
        {
            TargetFramework = targetFramework ?? throw new ArgumentNullException(nameof(targetFramework));
            RuntimeIdentifier = runtimeIdentifier;
        }

        public bool Equals(NuGetLockFileTarget other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return StringComparer.Ordinal.Equals(RuntimeIdentifier, other.RuntimeIdentifier)
                && NuGetFramework.Comparer.Equals(TargetFramework, other.TargetFramework)
                && EqualityUtility.SequenceEqualWithNullCheck(Dependencies, other.Dependencies);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as NuGetLockFileTarget);
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();

            combiner.AddObject(TargetFramework);
            combiner.AddObject(RuntimeIdentifier);
            combiner.AddSequence(Dependencies);

            return combiner.CombinedHash;
        }

        /// <summary>
        /// Return graph name in the form of {framework}/{RID}
        /// </summary>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Parse a target graph name in the form of {framework}/{RID}
        /// </summary>
        public static NuGetLockFileTarget Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var parts = s.Split('/');

            if (parts.Length > 2)
            {
                throw new ArgumentException(nameof(s));
            }

            return new NuGetLockFileTarget(
                targetFramework: NuGetFramework.Parse(parts[0], DefaultFrameworkNameProvider.Instance),
                runtimeIdentifier: parts.Length == 2 ? parts[1] : null);
        }

        private static string GetNameString(string framework, string runtime)
        {
            if (!string.IsNullOrEmpty(runtime))
            {
                return $"{framework}/{runtime}";
            }

            return framework;
        }
    }
}
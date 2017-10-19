// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.Packaging.Core
{
    /// <summary>
    /// Allows package modifications.
    /// </summary>
    public interface IPackageCoreWriter
    {
        /// <summary>
        /// Remove if the path exists.
        /// </summary>
        /// <param name="path">Relative file path in package.</param>
        /// <param name="token">CancellationToken</param>
        /// <remarks>Empty directories will be removed upon removing a file.</remarks>
        Task RemoveAsync(string path, CancellationToken token);

        /// <summary>
        /// Adds or replaces a file in the package.
        /// </summary>
        /// <param name="path">Relative file path in package.</param>
        /// <param name="stream">New file contents.</param>
        /// <param name="token">CancellationToken</param>
        /// <remarks>Directories will be created for new paths.</remarks>
        Task AddAsync(string path, Stream stream, CancellationToken token);
    }
}
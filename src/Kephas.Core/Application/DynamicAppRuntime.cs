﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Kephas.Collections;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// An application application runtime loading dynamically assemblies from the application localtion.
    /// </summary>
    public class DynamicAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAppRuntime"/> class.
        /// </summary>
        /// <param name="assemblyLoader">Optional. The assembly loader.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="defaultAssemblyFilter">Optional. The default assembly filter.</param>
        /// <param name="appLocation">Optional. The application location.</param>
        public DynamicAppRuntime(IAssemblyLoader assemblyLoader = null, ILogManager logManager = null, Func<AssemblyName, bool> defaultAssemblyFilter = null, string appLocation = null)
            : base(assemblyLoader, logManager, defaultAssemblyFilter, appLocation)
        {
        }

        /// <summary>
        /// Computes the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies.</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        protected override IEnumerable<Assembly> ComputeAppAssemblies(Func<AssemblyName, bool> assemblyFilter)
        {
            var assemblies = base.ComputeAppAssemblies(assemblyFilter).ToList();
            this.AddAdditionalAssemblies(assemblies, assemblyFilter);
            return assemblies;
        }

        /// <summary>
        /// Adds additional assemblies to the ones already collected.
        /// </summary>
        /// <param name="assemblies">The collected assemblies.</param>
        /// <param name="assemblyFilter">A filter for the assemblies.</param>
        protected virtual void AddAdditionalAssemblies(IList<Assembly> assemblies, Func<AssemblyName, bool> assemblyFilter)
        {
            // load all the assemblies found in the application directories which are not already loaded.
            var directories = this.GetAppAssemblyDirectories();
            foreach (var directory in directories.Where(d => !string.IsNullOrEmpty(d)))
            {
                var loadedAssemblyFiles = assemblies.Where(a => !a.IsDynamic).Select(this.GetFileName).Select(f => f.ToLowerInvariant());
                var assemblyFiles = this.EnumerateFiles(directory, AssemblyFileSearchPattern).Select(Path.GetFileName);
                var assemblyFilesToLoad = assemblyFiles
                                            .Where(f => !loadedAssemblyFiles.Contains(f.ToLowerInvariant()))
                                            .Where(f => assemblyFilter(this.GetAssemblyNameFromAssemblyFileName(f)));
                assemblies.AddRange(assemblyFilesToLoad
                                        .Select(f => this.AssemblyLoader.LoadAssemblyFromPath(Path.Combine(directory, f)))
                                        .Where(a => assemblyFilter(a.GetName())));
            }
        }

        /// <summary>
        /// Enumerates the files in the provided directory.
        /// </summary>
        /// <param name="directory">Pathname of the directory.</param>
        /// <param name="filePattern">A pattern specifying the files to retrieve.</param>
        /// <returns>
        /// An enumeration of file names.
        /// </returns>
        protected virtual IEnumerable<string> EnumerateFiles(string directory, string filePattern)
        {
            return Directory.EnumerateFiles(directory, filePattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Gets the directories where the application assemblies can be found.
        /// </summary>
        /// <remarks>
        /// Note for inheritors: This method can be overridden to provide additional directories
        /// where assemblies can be loaded from, like in the case of plugin architectures.
        /// </remarks>
        /// <returns>
        /// A directory enumeration.
        /// </returns>
        protected virtual IEnumerable<string> GetAppAssemblyDirectories()
        {
            return new[] { this.GetAppLocation() };
        }

        /// <summary>
        /// Gets the file name of the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The assembly file name.
        /// </returns>
        protected virtual string GetFileName(Assembly assembly)
        {
            return Path.GetFileName(assembly.GetFilePath());
        }

        /// <summary>
        /// Gets the assembly name from the assembly file name.
        /// </summary>
        /// <param name="f">The format string.</param>
        /// <returns>
        /// The assembly name.
        /// </returns>
        protected AssemblyName GetAssemblyNameFromAssemblyFileName(string f)
        {
            return new AssemblyName(f.Substring(0, f.Length - AssemblyFileExtension.Length));
        }
    }
}
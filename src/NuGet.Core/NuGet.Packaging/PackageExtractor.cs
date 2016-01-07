// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging.Core;

namespace NuGet.Packaging
{
    public static class PackageExtractor
    {
        public static async Task<IEnumerable<string>> ExtractPackageAsync(
            Stream packageStream,
            PackagePathResolver packagePathResolver,
            PackageExtractionContext packageExtractionContext,
            PackageSaveModes packageSaveMode,
            CancellationToken token)
        {
            if (packageStream == null)
            {
                throw new ArgumentNullException(nameof(packageStream));
            }

            if (!packageStream.CanSeek)
            {
                throw new ArgumentException(Strings.PackageStreamShouldBeSeekable);
            }

            if (packagePathResolver == null)
            {
                throw new ArgumentNullException("packagePathResolver");
            }

            var filesAdded = new List<string>();

            // TODO: Need to handle PackageSaveMode
            // TODO: Support overwriting files also?
            var nupkgStartPosition = packageStream.Position;
            var packageReader = new PackageArchiveReader(packageStream);

            var packageIdentityFromNuspec = packageReader.GetIdentity();

            var packageDirectoryInfo = Directory.CreateDirectory(packagePathResolver.GetInstallPath(packageIdentityFromNuspec));
            var packageDirectory = packageDirectoryInfo.FullName;

            var packageFiles = packageReader.GetPackageFiles(packageSaveMode);
            filesAdded.AddRange(await packageReader.CopyFilesAsync(packageDirectory, packageFiles, token));

            var nupkgFilePath = Path.Combine(packageDirectory, packagePathResolver.GetPackageFileName(packageIdentityFromNuspec));
            if (packageSaveMode.HasFlag(PackageSaveModes.Nupkg))
            {
                // During package extraction, nupkg is the last file to be created
                // Since all the packages are already created, the package stream is likely positioned at its end
                // Reset it to the nupkgStartPosition
                packageStream.Seek(nupkgStartPosition, SeekOrigin.Begin);
                filesAdded.Add(await PackageHelper.CreatePackageFileAsync(nupkgFilePath, packageStream, token));
            }

            // Now, copy satellite files unless requested to not copy them
            if (packageExtractionContext == null
                || packageExtractionContext.CopySatelliteFiles)
            {
                filesAdded.AddRange(await CopySatelliteFilesAsync(packageReader, packagePathResolver, packageSaveMode, token));
            }

            return filesAdded;
        }

        public static async Task<IEnumerable<string>> ExtractPackageAsync(
            PackageReaderBase packageReader,
            Stream packageStream,
            PackagePathResolver packagePathResolver,
            PackageExtractionContext packageExtractionContext,
            PackageSaveModes packageSaveMode,
            CancellationToken token)
        {
            if (packageStream == null)
            {
                throw new ArgumentNullException(nameof(packageStream));
            }

            if (packagePathResolver == null)
            {
                throw new ArgumentNullException(nameof(packagePathResolver));
            }

            // TODO: Need to handle PackageSaveMode
            // TODO: Support overwriting files also?
            var nupkgStartPosition = packageStream.Position;
            var filesAdded = new List<string>();

            var packageIdentityFromNuspec = packageReader.GetIdentity();

            var packageDirectoryInfo = Directory.CreateDirectory(packagePathResolver.GetInstallPath(packageIdentityFromNuspec));
            var packageDirectory = packageDirectoryInfo.FullName;

            var packageFiles = packageReader.GetPackageFiles(packageSaveMode);
            filesAdded.AddRange(await packageReader.CopyFilesAsync(packageDirectory, packageFiles, token));

            var nupkgFilePath = Path.Combine(packageDirectory, packagePathResolver.GetPackageFileName(packageIdentityFromNuspec));
            if (packageSaveMode.HasFlag(PackageSaveModes.Nupkg))
            {
                // During package extraction, nupkg is the last file to be created
                // Since all the packages are already created, the package stream is likely positioned at its end
                // Reset it to the nupkgStartPosition
                if (packageStream.Position != 0)
                {
                    if (!packageStream.CanSeek)
                    {
                        throw new ArgumentException(Strings.PackageStreamShouldBeSeekable);
                    }

                    packageStream.Position = 0;
                }

                filesAdded.Add(await PackageHelper.CreatePackageFileAsync(nupkgFilePath, packageStream, token));
            }

            // Now, copy satellite files unless requested to not copy them
            if (packageExtractionContext == null || packageExtractionContext.CopySatelliteFiles)
            {
                PackageIdentity runtimeIdentity;
                string packageLanguage;

                var nuspecReader = new NuspecReader(packageReader.GetNuspec());
                var isSatellitePackage = PackageHelper.IsSatellitePackage(nuspecReader, out runtimeIdentity, out packageLanguage);

                // Short-circuit this if the package is not a satellite package.
                if (isSatellitePackage)
                {
                    filesAdded.AddRange(await CopySatelliteFilesAsync(packageReader, packagePathResolver, packageSaveMode, token));
                }
            }

            return filesAdded;
        }

        public static async Task<IEnumerable<string>> CopySatelliteFilesAsync(
            PackageIdentity packageIdentity, 
            PackagePathResolver packagePathResolver,
            PackageSaveModes packageSaveMode, 
            CancellationToken token)
        {
            if (packageIdentity == null)
            {
                throw new ArgumentNullException(nameof(packageIdentity));
            }

            if (packagePathResolver == null)
            {
                throw new ArgumentNullException(nameof(packagePathResolver));
            }

            var satelliteFilesCopied = Enumerable.Empty<string>();

            var nupkgFilePath = packagePathResolver.GetInstalledPackageFilePath(packageIdentity);
            if (File.Exists(nupkgFilePath))
            {
                using (var packageStream = File.OpenRead(nupkgFilePath))
                {
                    var packageReader = new PackageArchiveReader(packageStream);
                    return await CopySatelliteFilesAsync(packageReader, packagePathResolver, packageSaveMode, token);
                }
            }

            return satelliteFilesCopied;
        }

        private static async Task<IEnumerable<string>> CopySatelliteFilesAsync(
            PackageReaderBase packageReader,
            PackagePathResolver packagePathResolver,
            PackageSaveModes packageSaveMode,
            CancellationToken token)
        {
            if (packageReader == null)
            {
                throw new ArgumentNullException(nameof(packageReader));
            }

            if (packagePathResolver == null)
            {
                throw new ArgumentNullException(nameof(packagePathResolver));
            }

            var satelliteFilesCopied = Enumerable.Empty<string>();

            string runtimePackageDirectory;
            var satelliteFiles = PackageHelper
                .GetSatelliteFiles(packageReader, packagePathResolver, out runtimePackageDirectory)
                .Where(file => PackageHelper.IsPackageFile(file, packageSaveMode))
                .ToList();
            if (satelliteFiles.Any())
            {
                // Now, add all the satellite files collected from the package to the runtime package folder(s)
                satelliteFilesCopied = await packageReader.CopyFilesAsync(runtimePackageDirectory, satelliteFiles, token);
            }

            return satelliteFilesCopied;
        }
    }
}

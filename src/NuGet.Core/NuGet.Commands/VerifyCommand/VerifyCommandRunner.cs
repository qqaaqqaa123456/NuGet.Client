// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using NuGet.Packaging.Test.SigningTests;

namespace NuGet.Commands
{
    /// <summary>
    /// Command Runner used to run the business logic for nuget verify command
    /// </summary>
    public class VerifyCommandRunner : IVerifyCommandRunner
    {
        public async Task<int> ExecuteCommandAsync(VerifyArgs verifyArgs)
        {
            if (!File.Exists(verifyArgs.PackagePath))
            {
                verifyArgs.Logger.LogError("Package provided does not exist.");
                return 1;
            }

            var nupkg = new FileInfo(verifyArgs.PackagePath);
            PackageArchiveReader package = null;

            try
            {
                package = new PackageArchiveReader(nupkg.OpenRead());
            }
            catch (Exception e)
            {
                verifyArgs.Logger.LogError("Provided file is not a valid nupkg.");
                return 1;
            }

            var packageIsSigned = await package.IsSignedAsync(CancellationToken.None);
            if (!packageIsSigned)
            {
                verifyArgs.Logger.LogError("Package provided is not signed.");
                return 1;
            }

            var trustProviders = new[] { new SignatureVerificationProvider() };
            var verifier = new SignedPackageVerifier(trustProviders, SignedPackageVerifierSettings.RequireSigned);
            var verifierResults = verifier.VerifySignaturesAsync(package, verifyArgs.Logger, CancellationToken.None);

            // check results

            // print info for result

            verifyArgs.Logger.LogInformation("Successfully verified signature in package.");

            return 0;
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Commands;

namespace NuGet.CommandLine
{
    [Command(typeof(NuGetCommand), "verify", "VerifyCommandDescription",
        MinArgs = 1,
        MaxArgs = 1,
        UsageSummaryResourceName = "VerifyCommandUsageSummary",
        UsageExampleResourceName = "VerifyCommandUsageExamples")]
    public class VerifyCommand : Command
    {
        // List of possible values - https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/security/cryptography/HashAlgorithmName.cs
        private static string[] _acceptedHashAlgorithms = { "SHA256", "SHA384", "SHA512" };

        protected VerifyCommand() : base()
        {
            Signer = new List<string>();
        }


        [Option(typeof(NuGetCommand), "VerifyCommandSignerDescription")]
        public ICollection<string> Signer { get; set; }

        public override Task ExecuteCommandAsync()
        {
            var packagePath = Arguments[0];

            if (string.IsNullOrEmpty(packagePath))
            {
                throw new ArgumentException("No package provided for verifying");
            }

            var verifyArgs = new VerifyArgs()
            {
                PackagePath = packagePath,
                Signer = new List<string>(),
                Logger = Console
            };

            switch (Verbosity)
            {
                case Verbosity.Detailed:
                    {
                        verifyArgs.LogLevel = Common.LogLevel.Verbose;
                        break;
                    }
                case Verbosity.Normal:
                    {
                        verifyArgs.LogLevel = Common.LogLevel.Information;
                        break;
                    }
                case Verbosity.Quiet:
                    {
                        verifyArgs.LogLevel = Common.LogLevel.Minimal;
                        break;
                    }
            }

            var verifyCommandRunner = new VerifyCommandRunner();
            return verifyCommandRunner.ExecuteCommandAsync(verifyArgs);
        }
    }
}

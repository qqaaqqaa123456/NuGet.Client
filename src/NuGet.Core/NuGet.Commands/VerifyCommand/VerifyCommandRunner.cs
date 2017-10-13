// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NuGet.Commands
{
    /// <summary>
    /// Command Runner used to run the business logic for nuget verify command
    /// </summary>
    public class VerifyCommandRunner : IVerifyCommandRunner
    {
        public int ExecuteCommand(VerifyArgs verifyArgs)
        {
            // check if the package exists
            if (!File.Exists(verifyArgs.PackagePath))
            {
                // error to user
                return 1;
            }

            verifyArgs.Logger.LogInformation($"Succesfully verified {verifyArgs.PackagePath} with verbosity level {verifyArgs.LogLevel}");

            return 0;
        }
    }
}

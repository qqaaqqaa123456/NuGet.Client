// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.ProjectModel;
using Xunit;
using FluentAssertions;
using System.Diagnostics;

namespace NuGet.Commands.Test
{
    public class TransitiveNoWarnUtilsTests
    {

        // Tests for TransitiveNoWarnUtils.ExtractPathNoWarnProperties
        [Fact]
        public void ExtractPathNoWarnProperties_ReturnsEmptySetIfPathPropertiesAreNull()
        {
            // Arrange & Act
            var extractedNoWarnSet = TransitiveNoWarnUtils.ExtractPathNoWarnProperties(null, "test_id");

            // Assert
            extractedNoWarnSet.Should().NotBeNull();
            extractedNoWarnSet.Should().BeEmpty();
        }

        [Fact]
        public void ExtractPathNoWarnProperties_CorrectlyReadsProjectWideNoWarns()
        {
            // Arrange
            var noWarnSet = new HashSet<NuGetLogCode> { NuGetLogCode.NU1601, NuGetLogCode.NU1603 };
            var warningsAsErrorSet = new HashSet<NuGetLogCode> { };

            var projectWideWarningProperties = new WarningProperties(
                warningsAsErrors: warningsAsErrorSet,
                noWarn: noWarnSet,
                allWarningsAsErrors: false
                );

            var warningPropertiesCollection = new WarningPropertiesCollection(
               projectWideWarningProperties: projectWideWarningProperties,
               packageSpecificWarningProperties: null,
               projectFrameworks: null
                );

            // Act
            var extractedNoWarnSet = TransitiveNoWarnUtils.ExtractPathNoWarnProperties(warningPropertiesCollection, "test_id");

            // Assert
            extractedNoWarnSet.Should().NotBeNullOrEmpty();
            extractedNoWarnSet.Should().BeEquivalentTo(noWarnSet);
        }

        [Fact]
        public void ExtractPathNoWarnProperties_CorrectlyReadsPackageSpecificNoWarns()
        {
            // Arrange
            var packageId = "test_package";
            var framework = NuGetFramework.Parse("net461");
            var expectedNoWarnSet = new HashSet<NuGetLogCode> { NuGetLogCode.NU1603, NuGetLogCode.NU1605 };
            var noWarnSet = new HashSet<NuGetLogCode> { };
            var warningsAsErrorSet = new HashSet<NuGetLogCode> { };

            var projectWideWarningProperties = new WarningProperties(
                warningsAsErrors: warningsAsErrorSet,
                noWarn: noWarnSet,
                allWarningsAsErrors: false
                );

            var packageSpecificWarningProperties = new PackageSpecificWarningProperties();
            packageSpecificWarningProperties.Add(NuGetLogCode.NU1603, packageId, framework);
            packageSpecificWarningProperties.Add(NuGetLogCode.NU1605, packageId, framework);
            packageSpecificWarningProperties.Add(NuGetLogCode.NU1701, "other_package", framework);

            var warningPropertiesCollection = new WarningPropertiesCollection(
               projectWideWarningProperties: projectWideWarningProperties,
               packageSpecificWarningProperties: packageSpecificWarningProperties,
               projectFrameworks: null
                );

            // Act
            var extractedNoWarnSet = TransitiveNoWarnUtils.ExtractPathNoWarnProperties(warningPropertiesCollection, packageId);

            // Assert
            extractedNoWarnSet.Should().NotBeNullOrEmpty();
            extractedNoWarnSet.Should().BeEquivalentTo(expectedNoWarnSet);
        }


        [Fact]
        public void ExtractPathNoWarnProperties_CorrectlyReadsPackageSpecificAndProjectWideNoWarns()
        {
            // Arrange
            var packageId = "test_package";
            var framework = NuGetFramework.Parse("net461");
            var expectedNoWarnSet = new HashSet<NuGetLogCode> { NuGetLogCode.NU1601 , NuGetLogCode.NU1603, NuGetLogCode.NU1605 };
            var noWarnSet = new HashSet<NuGetLogCode> { NuGetLogCode.NU1601, NuGetLogCode.NU1605 };
            var warningsAsErrorSet = new HashSet<NuGetLogCode> { };

            var projectWideWarningProperties = new WarningProperties(
                warningsAsErrors: warningsAsErrorSet,
                noWarn: noWarnSet,
                allWarningsAsErrors: false
                );

            var packageSpecificWarningProperties = new PackageSpecificWarningProperties();
            packageSpecificWarningProperties.Add(NuGetLogCode.NU1603, packageId, framework);
            packageSpecificWarningProperties.Add(NuGetLogCode.NU1605, packageId, framework);
            packageSpecificWarningProperties.Add(NuGetLogCode.NU1701, "other_package", framework);

            var warningPropertiesCollection = new WarningPropertiesCollection(
               projectWideWarningProperties: projectWideWarningProperties,
               packageSpecificWarningProperties: packageSpecificWarningProperties,
               projectFrameworks: null
                );

            // Act
            var extractedNoWarnSet = TransitiveNoWarnUtils.ExtractPathNoWarnProperties(warningPropertiesCollection, packageId);

            // Assert
            extractedNoWarnSet.Should().NotBeNullOrEmpty();
            extractedNoWarnSet.Should().BeEquivalentTo(expectedNoWarnSet);
        }


        // Tests for TransitiveNoWarnUtils.TryMergeNullObjects
        [Fact]
        public void TryMergeNullObjects_ReturnsNullIfBothAreNull()
        {
            // Arrange
            object mergedObject;
            object first = null;
            object second = null;

            // Act
            var success = TransitiveNoWarnUtils.TryMergeNullObjects(first, second, out mergedObject);

            // Assert
            success.Should().BeTrue();
            mergedObject.Should().BeNull();
        }

        [Fact]
        public void TryMergeNullObjects_ReturnsFirstIfNotNull()
        {
            // Arrange
            object mergedObject;
            object first = new object();
            object second = null;

            // Act
            var success = TransitiveNoWarnUtils.TryMergeNullObjects(first, second, out mergedObject);

            // Assert
            success.Should().BeTrue();
            mergedObject.Should().Be(first);
        }

        [Fact]
        public void TryMergeNullObjects_ReturnsSecondIfNotNull()
        {
            // Arrange
            object mergedObject;
            object first = null;
            object second = new object();

            // Act
            var success = TransitiveNoWarnUtils.TryMergeNullObjects(first, second, out mergedObject);

            // Assert
            success.Should().BeTrue();
            mergedObject.Should().Be(second);
        }

        [Fact]
        public void TryMergeNullObjects_ReturnsFailureIfNoneNull()
        {
            // Arrange
            object mergedObject;
            object first = new object();
            object second = new object();

            // Act
            var success = TransitiveNoWarnUtils.TryMergeNullObjects(first, second, out mergedObject);

            // Assert
            success.Should().BeFalse();
            mergedObject.Should().BeNull();
        }

        // Tests for TransitiveNoWarnUtils.MergePackageSpecificWarningProperties
        [Fact]
        public void MergePackageSpecificWarningProperties_ReturnsNullIfBothAreNull()
        {
            // Arrange
            PackageSpecificWarningProperties first = null;
            PackageSpecificWarningProperties second = null;

            // Act
            var merged = TransitiveNoWarnUtils.MergePackageSpecificWarningProperties(first, second);

            // Assert
            merged.Should().BeNull();
        }

        [Fact]
        public void MergePackageSpecificWarningProperties_ReturnsFirstIfNotNull()
        {
            // Arrange
            PackageSpecificWarningProperties first = new PackageSpecificWarningProperties();
            PackageSpecificWarningProperties second = null;

            // Act
            var merged = TransitiveNoWarnUtils.MergePackageSpecificWarningProperties(first, second);

            // Assert
            merged.Should().NotBeNull();
            merged.Should().Be(first);
        }

        [Fact]
        public void MergePackageSpecificWarningProperties_MergesEmptyCollections()
        {
            // Arrange
            var first = new PackageSpecificWarningProperties();
            var second = new PackageSpecificWarningProperties();

            // Act
            var merged = TransitiveNoWarnUtils.MergePackageSpecificWarningProperties(first, second);

            // Assert
            merged.Should().NotBeNull();
            merged.Properties.Should().BeNull();
        }

        [Fact]
        public void MergePackageSpecificWarningProperties_MergesNonEmptyCollections()
        {
            // Arrange
            var packageId1 = "test_id1";
            var packageId2 = "test_id2";
            var net461 = NuGetFramework.Parse("net461");
            var netcoreapp = NuGetFramework.Parse("netcoreapp2.0");
            var expectedResult = new PackageSpecificWarningProperties();
            expectedResult.AddRangeOfCodes(
                new List<NuGetLogCode> { NuGetLogCode.NU1601, NuGetLogCode.NU1605 },
                packageId1,
                net461);
            expectedResult.AddRangeOfFrameworks(
                NuGetLogCode.NU1701,
                packageId2,
                new List<NuGetFramework> { net461, netcoreapp });
            expectedResult.AddRangeOfFrameworks(
                NuGetLogCode.NU1701,
                packageId1,
                new List<NuGetFramework> { net461, netcoreapp });
            expectedResult.AddRangeOfFrameworks(
                NuGetLogCode.NU1701,
                packageId2,
                new List<NuGetFramework> { net461, netcoreapp });
            expectedResult.AddRangeOfFrameworks(
                NuGetLogCode.NU1604,
                packageId1,
                new List<NuGetFramework> { net461, netcoreapp });


            var first = new PackageSpecificWarningProperties();
            first.AddRangeOfCodes(
                new List<NuGetLogCode> { NuGetLogCode.NU1601, NuGetLogCode.NU1605 },
                packageId1,
                net461);
            first.AddRangeOfFrameworks(
                NuGetLogCode.NU1701,
                packageId2,
                new List<NuGetFramework> { net461, netcoreapp });
            first.AddRangeOfFrameworks(
                NuGetLogCode.NU1701,
                packageId1,
                new List<NuGetFramework> { net461, netcoreapp });

            var second = new PackageSpecificWarningProperties();
            second.AddRangeOfFrameworks(
                NuGetLogCode.NU1701,
                packageId2,
                new List<NuGetFramework> { net461, netcoreapp });
            second.AddRangeOfFrameworks(
                NuGetLogCode.NU1604,
                packageId1,
                new List<NuGetFramework> { net461, netcoreapp });


            // Act
            var merged = TransitiveNoWarnUtils.MergePackageSpecificWarningProperties(first, second);

            // Assert
            merged.Should().NotBeNull();
            merged.Properties.Should().NotBeNull();
            merged.ShouldBeEquivalentTo(expectedResult);
        }
    }
}

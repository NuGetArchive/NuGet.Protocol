// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using NuGet.Protocol.Core.Types;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace Client.V3Test
//{
//    public class V3TotalsStatsResourceTests : TestBase
//    {
//        [Fact]
//        public async Task GetTotalStatsHasExpectedProperties()
//        {
//            var resource = await SourceRepository.GetResourceAsync<V3TotalsStatsResource>();
//            var result = await resource.GetTotalStatsAsync(CancellationToken.None);

//            Assert.NotNull(result);
//            Assert.NotNull(result["uniquePackages"]);
//            Assert.NotNull(result["totalPackages"]);
//            Assert.NotNull(result["downloads"]);
//            Assert.NotNull(result["operationTotals"]);
//            Assert.NotNull(result["lastUpdateDateUtc"]);
//        }
//    }
//}﻿
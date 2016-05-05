﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Protocol;
using Microsoft.Azure.Batch.Protocol.Models;
using Microsoft.WindowsAzure.Commands.ScenarioTest;
using Moq;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Rest.Azure;
using Xunit;
using BatchClient = Microsoft.Azure.Commands.Batch.Models.BatchClient;

namespace Microsoft.Azure.Commands.Batch.Test.Jobs
{
    public class RemoveBatchJobCommandTests : WindowsAzure.Commands.Test.Utilities.Common.RMTestBase
    {
        private RemoveBatchJobCommand cmdlet;
        private Mock<BatchClient> batchClientMock;
        private Mock<ICommandRuntime> commandRuntimeMock;

        public RemoveBatchJobCommandTests(Xunit.Abstractions.ITestOutputHelper output)
        {
            ServiceManagemenet.Common.Models.XunitTracingInterceptor.AddToContext(new ServiceManagemenet.Common.Models.XunitTracingInterceptor(output));
            batchClientMock = new Mock<BatchClient>();
            commandRuntimeMock = new Mock<ICommandRuntime>();
            cmdlet = new RemoveBatchJobCommand()
            {
                CommandRuntime = commandRuntimeMock.Object,
                BatchClient = batchClientMock.Object,
            };
        }

        [Fact]
        [Trait(Category.AcceptanceType, Category.CheckIn)]
        public void RemoveBatchJobParametersTest()
        {
            // Setup cmdlet without the required parameters
            BatchAccountContext context = BatchTestHelpers.CreateBatchContextWithKeys();
            cmdlet.BatchContext = context;

            // Setup cmdlet to skip confirmation popup
            cmdlet.Force = true;
            commandRuntimeMock.Setup(f => f.ShouldProcess(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            Assert.Throws<ArgumentNullException>(() => cmdlet.ExecuteCmdlet());

            cmdlet.Id = "job-1";

            // Don't go to the service on a Delete CloudJob call
            RequestInterceptor interceptor = BatchTestHelpers.CreateFakeServiceResponseInterceptor<JobDeleteOptions, AzureOperationHeaderResponse<JobDeleteHeaders>>();
            cmdlet.AdditionalBehaviors = new List<BatchClientBehavior>() { interceptor };

            // Verify no exceptions when required parameters are set
            cmdlet.ExecuteCmdlet();
        }
    }
}

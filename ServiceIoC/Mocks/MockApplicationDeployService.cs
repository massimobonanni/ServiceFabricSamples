// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace PartyCluster.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MockApplicationDeployService : IApplicationDeployService
    {
        public MockApplicationDeployService()
        {
            this.GetApplicationDeploymentsAsyncFunc = (cluster, port) => Task.FromResult(Enumerable.Empty<ApplicationView>());
        }

        public Func<string, int, Task<IEnumerable<ApplicationView>>> GetApplicationDeploymentsAsyncFunc { get; set; }

        public Task<int> GetApplicationCountAsync(string clusterAddress, int clusterPort)
        {
            return Task.FromResult(0);
        }

        public Task<IEnumerable<ApplicationView>> GetApplicationDeploymentsAsync(string clusterAddress, int port)
        {
            return this.GetApplicationDeploymentsAsyncFunc(clusterAddress, port);
        }

        public Task<int> GetServiceCountAsync(string clusterAddress, int clusterPort)
        {
            return Task.FromResult(0);
        }

        public Task<ApplicationDeployStatus> GetStatusAsync(Guid deployId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> QueueApplicationDeploymentAsync(string clusterAddress, int clusterPort)
        {
            return Task.FromResult(Enumerable.Repeat(Guid.NewGuid(), 1));
        }
    }
}
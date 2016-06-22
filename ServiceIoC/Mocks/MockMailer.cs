// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace PartyCluster.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PartyCluster.Domain;

    public class MockMailer : ISendMail
    {
        public MockMailer()
        {
            this.SendJoinMailFunc = (receipientAddress, clusterAddress, userPort, timeRemaining, clusterExpiration, links) => Task.FromResult(true);
        }

        public Func<string, string, int, TimeSpan, DateTimeOffset, IEnumerable<HyperlinkView>, Task> SendJoinMailFunc { get; set; }

        public Task SendJoinMail(
            string receipientAddress, string clusterAddress, int userPort, TimeSpan timeRemaining, DateTimeOffset clusterExpiration,
            IEnumerable<HyperlinkView> links)
        {
            return this.SendJoinMailFunc(receipientAddress, clusterAddress, userPort, timeRemaining, clusterExpiration, links);
        }
    }
}
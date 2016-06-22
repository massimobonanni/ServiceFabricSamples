// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Mocks
{
    using System;
    using Microsoft.ServiceFabric.Data;

    internal static class ConditionalResultActivator
    {
        public static ConditionalValue<T> Create<T>(bool result, T value)
        {
            return (ConditionalValue<T>) Activator.CreateInstance(
                typeof(ConditionalValue<T>),
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                null,
                new object[] {result, value},
                null);
        }
    }
}
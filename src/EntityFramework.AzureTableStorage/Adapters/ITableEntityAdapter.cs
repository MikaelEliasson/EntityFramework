// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Data.Entity.AzureTableStorage.Adapters
{
    public interface ITableEntityAdapter<out T> : ITableEntity
    {
        T Entity { get; }
    }
}

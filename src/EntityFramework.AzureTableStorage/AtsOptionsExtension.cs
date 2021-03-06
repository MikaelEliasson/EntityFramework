﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.Data.Entity.AzureTableStorage
{
    public class AtsOptionsExtension : DbContextOptionsExtension
    {
        public virtual string ConnectionString { get; internal set; }
        public virtual bool UseBatching { get; internal set; }

        public AtsOptionsExtension()
        {
            UseBatching = false;
        }

        protected override void ApplyServices(EntityServicesBuilder builder)
        {
            builder.AddAzureTableStorage();
        }
    }
}

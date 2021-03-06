﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.AzureTableStorage.Utilities;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;

namespace Microsoft.Data.Entity.AzureTableStorage.Query
{
    public class AtsQueryCompilationContext : QueryCompilationContext
    {
        public virtual TableFilterFactory TableFilterFactory { get; private set; }

        public AtsQueryCompilationContext([NotNull] IModel model, [NotNull] TableFilterFactory tableFilterFactory)
            : base(model, new LinqOperatorProvider(), new ResultOperatorHandler())
        {
            Check.NotNull(tableFilterFactory, "tableFilterFactory");
            TableFilterFactory = tableFilterFactory;
        }

        public override EntityQueryModelVisitor CreateQueryModelVisitor()
        {
            return new AtsQueryModelVisitor(this);
        }
    }
}

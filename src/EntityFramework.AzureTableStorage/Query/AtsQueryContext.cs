﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.AzureTableStorage.Requests;
using Microsoft.Data.Entity.AzureTableStorage.Utilities;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace Microsoft.Data.Entity.AzureTableStorage.Query
{
    public class AtsQueryContext : QueryContext
    {
        private readonly AtsConnection _connection;

        private readonly ThreadSafeDictionaryCache<QueryKey, IEnumerable> _requestCache
            = new ThreadSafeDictionaryCache<QueryKey, IEnumerable>();

        public AtsQueryContext(
            [NotNull] IModel model,
            [NotNull] ILogger logger,
            [NotNull] StateManager stateManager,
            [NotNull] AtsConnection connection,
            [NotNull] AtsValueReaderFactory readerFactory)
            : base(model, logger, stateManager)
        {
            Check.NotNull(model, "model");
            Check.NotNull(logger, "logger");
            Check.NotNull(stateManager, "stateManager");
            Check.NotNull(readerFactory, "readerFactory");

            _connection = connection;
            ValueReaderFactory = readerFactory;
        }

        public virtual AtsConnection Connection
        {
            get { return _connection; }
        }

        public virtual AtsValueReaderFactory ValueReaderFactory { get; private set; }

        public virtual IEnumerable<TResult> GetOrAddQueryResults<TResult>([NotNull] QueryTableRequest<TResult> request)
        {
            Check.NotNull(request, "request");
            return _requestCache.GetOrAdd(new QueryKey(request.Table, request.Query),
                q => Connection
                    .ExecuteRequest(request, Logger)
                    .ToList() // prevent multiple execution
                ).Cast<TResult>();
        }

        private struct QueryKey
        {
            public readonly AtsTable Table;
            public readonly AtsTableQuery Query;

            public QueryKey(AtsTable table, AtsTableQuery query)
            {
                Table = table;
                Query = query;
            }

            public bool Equals(QueryKey other)
            {
                return Table.Equals(other.Table) && Query.Equals(other.Query);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                return obj is QueryKey && Equals((QueryKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Table.GetHashCode() * 397) ^ Query.GetHashCode();
                }
            }
        }
    }
}

﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Framework.Logging;

namespace Microsoft.Data.Entity.Relational.Query
{
    public class EnumerableMethodProvider : IEnumerableMethodProvider
    {
        public virtual MethodInfo QueryValues
        {
            get { return _queryValuesMethodInfo; }
        }

        private static readonly MethodInfo _queryValuesMethodInfo
            = typeof(EnumerableMethodProvider).GetTypeInfo()
                .GetDeclaredMethod("_QueryValues");

        [UsedImplicitly]
        private static IEnumerable<IValueReader> _QueryValues(
            QueryContext queryContext, CommandBuilder commandBuilder)
        {
            var relationalQueryContext = (RelationalQueryContext)queryContext;

            return new Enumerable<IValueReader>(
                relationalQueryContext.Connection,
                commandBuilder,
                r => relationalQueryContext.ValueReaderFactory.Create(r),
                queryContext.Logger);
        }

        public virtual MethodInfo QueryEntities
        {
            get { return _queryEntitiesMethodInfo; }
        }

        private static readonly MethodInfo _queryEntitiesMethodInfo
            = typeof(EnumerableMethodProvider).GetTypeInfo()
                .GetDeclaredMethod("_QueryEntities");

        [UsedImplicitly]
        private static IEnumerable<TEntity> _QueryEntities<TEntity>(
            QueryContext queryContext, CommandBuilder commandBuilder)
        {
            var relationalQueryContext = ((RelationalQueryContext)queryContext);

            return new Enumerable<TEntity>(
                relationalQueryContext.Connection,
                commandBuilder,
                r => (TEntity)queryContext.StateManager
                    .GetOrMaterializeEntry(
                        queryContext.Model.GetEntityType(typeof(TEntity)),
                        relationalQueryContext.ValueReaderFactory.Create(r)).Entity,
                queryContext.Logger);
        }

        private sealed class Enumerable<T> : IEnumerable<T>
        {
            private readonly RelationalConnection _connection;
            private readonly CommandBuilder _commandBuilder;
            private readonly Func<DbDataReader, T> _shaper;
            private readonly ILogger _logger;

            public Enumerable(
                RelationalConnection connection,
                CommandBuilder commandBuilder,
                Func<DbDataReader, T> shaper,
                ILogger logger)
            {
                _connection = connection;
                _commandBuilder = commandBuilder;
                _shaper = shaper;
                _logger = logger;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new Enumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private sealed class Enumerator : IEnumerator<T>
            {
                private readonly Enumerable<T> _enumerable;

                private DbCommand _command;
                private DbDataReader _reader;

                public Enumerator(Enumerable<T> enumerable)
                {
                    _enumerable = enumerable;
                }

                public bool MoveNext()
                {
                    if (_reader == null)
                    {
                        _enumerable._connection.Open();

                        _command = _enumerable._commandBuilder.Build(_enumerable._connection.DbConnection);

                        _enumerable._logger.WriteSql(_command.CommandText);

                        _reader = _command.ExecuteReader();
                    }

                    return _reader.Read();
                }

                public T Current
                {
                    get
                    {
                        if (_reader == null)
                        {
                            return default(T);
                        }

                        return _enumerable._shaper(_reader);
                    }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                public void Dispose()
                {
                    if (_reader != null)
                    {
                        _reader.Dispose();
                    }

                    if (_command != null)
                    {
                        _command.Dispose();
                    }

                    if (_enumerable._connection != null)
                    {
                        _enumerable._connection.Close();
                    }
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}

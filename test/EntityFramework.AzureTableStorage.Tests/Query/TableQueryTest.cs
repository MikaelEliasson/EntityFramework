﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.Data.Entity.AzureTableStorage.Query;
using Microsoft.Data.Entity.AzureTableStorage.Tests.Helpers;
using Xunit;

namespace Microsoft.Data.Entity.AzureTableStorage.Tests.Query
{
    public class TableQueryTest
    {
        private readonly AtsTableQuery _tableQuery;

        public TableQueryTest()
        {
            _tableQuery = new AtsTableQuery();
        }

        [Fact]
        public void It_does_lazy_eval_of_filters()
        {
            var testObj = new FieldType();
            var expression =
                Expression.MakeBinary(
                    ExpressionType.Equal,
                    Expression.MakeMemberAccess(
                        Expression.New(
                            typeof(PocoTestType)),
                        typeof(PocoTestType).GetProperty("Count")
                        ),
                    Expression.Field(
                        Expression.Constant(testObj),
                        typeof(FieldType).GetField("InstanceIntField")
                        )
                    );
            var filter = new TableFilterFactory().TryCreate(expression, PocoTestType.EntityType());
            _tableQuery.WithFilter(filter);

            testObj.InstanceIntField = -87;
            Assert.Equal("Count eq -87", _tableQuery.ToString());
        }

        internal class FieldType : PocoTestType
        {
            public FieldType()
            {
                InstanceIntField = 0;
            }

            public int InstanceIntField;
        }
    }
}

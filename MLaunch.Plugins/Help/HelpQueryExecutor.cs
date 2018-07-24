﻿using MLaunch.Core.QueryExecutors;
using MLaunch.Core.QueryExecutors.ListQuery;
using MUI;
using MUI.DI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MLaunch.Plugins.Help
{
    [Service]
    public class HelpQueryExecutor : ListQueryExecutor
    {
        [Dependency] public ResourceManager ResourceManager { get; set; }
        [Dependency] public IQueryExecutor[] QueryExecutors { get; set; }

        public override string Name => "Help";

        public override string Description => "Displays this listing";

        public override string ExampleUsage => "help";

        public override int Order => 999;

        [RunAfterInject]
        private void Init()
        {
            QueryResults = GetQueryResults(null);
        }

        public override bool TryHandle(string term) => string.IsNullOrWhiteSpace(term) || term.ContainsCaseInsensitive("help");

        public override IList<IListQueryResult> GetQueryResults(string term)
        {
            return QueryExecutors
                .OrderBy(qe => qe.Name)
                .Select(qe => (IListQueryResult)new ListQueryResult()
                {
                    Name = $"{qe.Name} (eg. '{qe.ExampleUsage}')",
                    Description = qe.Description
                })
                .ToList();
        }
    }
}
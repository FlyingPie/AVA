﻿namespace MLaunch.Core.QueryExecutors
{
    public interface IQueryExecutor
    {
        string Name { get; }

        string Description { get; }

        string ExampleUsage { get; }

        int Order { get; }

        bool TryHandle(string term);

        bool TryExecute(QueryContext query);

        void Draw();
    }
}
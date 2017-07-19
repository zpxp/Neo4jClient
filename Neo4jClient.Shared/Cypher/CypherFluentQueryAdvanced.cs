﻿using System;

namespace Neo4jClient.Cypher
{
    public class CypherFluentQueryAdvanced : ICypherFluentQueryAdvanced
    {
        private readonly IGraphClient client;
        private readonly QueryWriter queryWriter;

        public CypherFluentQueryAdvanced(IGraphClient client, QueryWriter queryWriter) 
        {
            this.client = client;
            this.queryWriter = queryWriter;
        }

        public ICypherFluentQuery<TResult> Return<TResult>(ReturnExpression returnExpression)
        {
            return Mutate<TResult>(w =>
            {
                w.ResultMode = returnExpression.ResultMode;
                w.ResultFormat = returnExpression.ResultFormat;
                w.AppendClause("RETURN " + returnExpression.Text);
            });
        }

        public ICypherFluentQuery<TResult> ReturnDistinct<TResult>(ReturnExpression returnExpression)
        {
            return Mutate<TResult>(w =>
            {
                w.ResultMode = returnExpression.ResultMode;
                w.ResultFormat = returnExpression.ResultFormat;
                w.AppendClause("RETURN distinct " + returnExpression.Text);
            });
        }

        public ICypherFluentQuery<TResult> SetClient<TResult>(IGraphClient graphClient)
        {
            if (!(client is IRawGraphClient))
                throw new ArgumentException("The supplied graph client also needs to implement IRawGraphClient", nameof(graphClient));

            return new CypherFluentQuery<TResult>(graphClient, queryWriter);
        }

        public ICypherFluentQuery SetClient(IGraphClient graphClient)
        {
            if (!(client is IRawGraphClient))
                throw new ArgumentException("The supplied graph client also needs to implement IRawGraphClient", nameof(graphClient));
            
            return new CypherFluentQuery(graphClient, queryWriter);
        }

        protected ICypherFluentQuery<TResult> Mutate<TResult>(Action<QueryWriter> callback)
        {
            var newWriter = queryWriter.Clone();
            callback(newWriter);
            return new CypherFluentQuery<TResult>(client, newWriter);
        }
    }
}

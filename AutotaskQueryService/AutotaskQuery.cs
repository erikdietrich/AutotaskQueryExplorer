using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AutotaskQueryService
{
    internal class AutotaskQuery
    {
        public const string RootNodeName = "queryxml";
        public const string EntityNodeName = "entity";
        public const string QueryNodeName = "query";
        public const string FieldNodeName = "field";
        public const string ExpressionNodeName = "expression";

        private readonly Dictionary<string, string> _operatorMappings = new Dictionary<string, string>() 
            {
                { "=", "equals"},
                { "<>", "notequal" },
                { ">", "greaterthan"},
                { "<", "lessthan" },
                { ">=", "greaterthanorequals"},
                { "<=", "lessthanorequals"},
            };

        private readonly List<XElement> _queryClauses = new List<XElement>();

        public string Entity { get; private set; }

        public AutotaskQuery(string entity)
        {
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentException("entity");

            Entity = entity;
            _queryClauses.Add(BuildQueryNode("id", "greaterthan", "0"));
        }

        public void SetWhereClause(string sqlClause)
        {
            _queryClauses.Clear();

            var tokens = sqlClause.Split(' ');
            string field = tokens[0];
            string op = _operatorMappings[tokens[1]];
            string value = GetValue(tokens);

            var queryNode = BuildQueryNode(field, op, value.Replace("'", string.Empty));
            _queryClauses.Add(queryNode);
        }

        private static string GetValue(string[] tokens)
        {
            if (tokens.Count() > 3)
                return String.Join(" ", tokens.Skip(2));
            else
                return tokens[2];
        }

        public override string ToString()
        {
            var entityNode = new XElement(EntityNodeName, Entity);

            var rootNode = new XElement(RootNodeName, entityNode, _queryClauses[0]);
            return rootNode.ToString();
        }

        private XElement BuildQueryNode(string fieldName, string op, string value)
        {
            var expressionNode = new XElement(ExpressionNodeName, value);
            expressionNode.SetAttributeValue("op", op);
            var fieldNode = new XElement(FieldNodeName, fieldName, expressionNode);
            return new XElement(QueryNodeName, fieldNode);
        }
    }
}

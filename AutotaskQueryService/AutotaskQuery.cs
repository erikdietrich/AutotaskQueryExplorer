using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public const string ConditionNodeName = "condition";

        public const string OperatorAttribute = "op";
        public const string Wildcard = "%";
        public const string BeginsWithOperator = "beginswith";
        public const string EndsWithOperator = "endswith";
        public const string ContainsOperator = "contains";

        private readonly Dictionary<string, string> _operatorMappings = new Dictionary<string, string>() 
            {
                { "=", "equals"},
                { "<>", "notequal" },
                { ">", "greaterthan"},
                { "<", "lessthan" },
                { ">=", "greaterthanorequals"},
                { "<=", "lessthanorequals"},
            };

        private readonly List<XElement> _conditionClauses = new List<XElement>();

        public string Entity { get; private set; }

        public AutotaskQuery(string entity)
        {
            if (string.IsNullOrEmpty(entity))
                throw new ArgumentException("entity");

            Entity = entity;
            _conditionClauses.Add(BuildConditionNode("id", _operatorMappings[">"], "0"));
        }

        public void SetWhereClause(string sqlClause)
        {
            _conditionClauses.Clear();
            foreach (var clause in Regex.Split(sqlClause, "AND", RegexOptions.IgnoreCase).Select(str => str.Trim()))
            {
                var tokens = clause.Split(' ');
                string field = tokens[0];
                string op = _operatorMappings[tokens[1]];
                string value = GetValue(tokens);

                var queryNode = BuildConditionNode(field, op, value.Replace("'", string.Empty));
                _conditionClauses.Add(queryNode);
            }
        }

        public override string ToString()
        {
            var entityNode = new XElement(EntityNodeName, Entity);
            var queryNode = new XElement(QueryNodeName, _conditionClauses);
            var rootNode = new XElement(RootNodeName, entityNode, queryNode);

            return rootNode.ToString();
        }

        private static string GetValue(string[] tokens)
        {
            if (tokens.Count() > 3)
                return String.Join(" ", tokens.Skip(2));
            else
                return tokens[2];
        }

        #region Could be a class?

        private static XElement BuildConditionNode(string fieldName, string op, string value)
        {
            var expressionNode = BuildExpressionNode(op, value);
            var fieldNode = new XElement(FieldNodeName, fieldName, expressionNode);
            return new XElement(ConditionNodeName, fieldNode);
        }

        private static XElement BuildExpressionNode(string op, string value)
        {
            if (value.StartsWith(Wildcard) && value.EndsWith(Wildcard))
                return BuildWildcardExpressionNode(ContainsOperator, value);
            if (value.StartsWith(Wildcard))
                return BuildWildcardExpressionNode(EndsWithOperator, value);
            else if (value.EndsWith(Wildcard))
                return BuildWildcardExpressionNode(BeginsWithOperator, value);
            else
                return BuildNormalExpressionNode(op, value);
        }

        private static XElement BuildWildcardExpressionNode(string op, string value)
        {
            var expressionNode = new XElement(ExpressionNodeName, value.Replace(Wildcard, string.Empty));
            expressionNode.SetAttributeValue(OperatorAttribute, op);
            return expressionNode;
        }

        private static XElement BuildNormalExpressionNode(string op, string value)
        {
            var expressionNode = new XElement(ExpressionNodeName, value);
            expressionNode.SetAttributeValue(OperatorAttribute, op);
            return expressionNode;
        }

        #endregion
    }
}

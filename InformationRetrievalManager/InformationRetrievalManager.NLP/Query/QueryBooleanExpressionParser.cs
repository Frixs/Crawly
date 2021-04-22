using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Expression recursive descent parser with logical (boolean) operations for <see cref="BooleanModel"/>.
    /// TODO: advanced/complex queries does not work to parse
    /// </summary>
    /// <remarks>
    /// Grammar: <br />
    ///     S -> E                  <br />
    ///     E -> T                  <br />
    ///     E -> T 'AND' E          <br />
    ///     E -> T 'OR' E           <br />
    ///     T -> F                  <br />
    ///     T -> '(' E ')'          <br />
    ///     T -> 'NOT' E            <br />
    ///     F -> @        => (Term) <br />
    /// </remarks>
    public sealed class QueryBooleanExpressionParser
    {
        #region Keyword Constants

        public const string KeywordAnd = "AND";
        public const string KeywordOr = "OR";
        public const string KeywordNot = "NOT";
        public const string KeywordLeftParen = "(";
        public const string KeywordRightParen = ")";

        #endregion

        #region Public Properties

        /// <summary>
        /// Rgex used in the tokenization.
        /// </summary>
        public string TokenizationRegex => $@"({KeywordAnd})|({KeywordOr})|({KeywordNot})|(\{KeywordLeftParen})|(\{KeywordRightParen})";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryBooleanExpressionParser()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tokenizes the text according to keywords (e.g. 'AND', 'OR', etc.) and keep them in the token array.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>Tokens</returns>
        /// <exception cref="ArgumentNullException">Invalid parameters</exception>
        public string[] Tokenize(string text)
        {
            if (text == null)
                throw new ArgumentNullException("Text not specified!");

            Regex rgx = new Regex(TokenizationRegex);
            return rgx.Split(text).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }

        /// <summary>
        /// Follows logically <see cref="Parse"/> but this is publicly visible for checking if the syntax is correct or not.
        /// </summary>
        /// <param name="tokens">The tokens</param>
        /// <returns><see langword="true"/> on correct syntax, otherwise <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException">Invalid parameters</exception>
        public bool CheckParse(string[] tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException("Tokens not specified!");

            int index = 0;

            try
            {
                ParseE(tokens, ref index); // Rule 1
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Parse tokens (tokenized text) according to the grammar.
        /// </summary>
        /// <param name="tokens">The tokens</param>
        /// <returns>Node for evaluation</returns>
        /// <exception cref="ArgumentNullException">Invalid parameters</exception>
        /// <exception cref="Exception">If parsing fails</exception>
        internal Node Parse(string[] tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException("Tokens not specified!");

            int index = 0;
            return ParseE(tokens, ref index); // Rule 1
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// E rule parse method
        /// </summary>
        /// <param name="tokens">The tokens</param>
        /// <param name="index">Token index</param>
        /// <returns>Node for further parsing</returns>
        /// <exception cref="Exception">If parsing fails</exception>
        private Node ParseE(string[] tokens, ref int index)
        {
            Node leftExp = ParseT(tokens, ref index);
            if (index >= tokens.Length) // Last token => Rule 2
                return leftExp;

            string token = tokens[index];
            if (token == KeywordAnd) // Rule 3
            {
                index++;
                Node rightExp = ParseE(tokens, ref index);
                return new AndNode(leftExp, rightExp);
            }
            else if (token == KeywordOr) // Rule 4
            {
                index++;
                Node rightExp = ParseE(tokens, ref index);
                return new OrNode(leftExp, rightExp);
            }
            else
            {
                throw new Exception($"Expected '{KeywordAnd}' or '{KeywordOr}' or EOF");
            }
        }

        /// <summary>
        /// T rule parse method
        /// </summary>
        /// <param name="tokens">The tokens</param>
        /// <param name="index">Token index</param>
        /// <returns>Node for further parsing</returns>
        /// <exception cref="Exception">If parsing fails</exception>
        private Node ParseT(string[] tokens, ref int index)
        {
            string token = tokens[index];
            if (token == KeywordLeftParen) // Rule 6
            {
                index++;
                Node node = ParseE(tokens, ref index);
                if (tokens[index] != KeywordRightParen)
                    throw new Exception($"Expected '{KeywordRightParen}'");

                index++; // Skip ')'
                return node;
            }
            else if (token == KeywordNot) // Rule 7
            {
                index++;
                Node node = ParseE(tokens, ref index);
                return new NotNode(node);
            }
            else
            {
                index++;
                return new TermNode(token); // Rule 5 (8)
            }
        }

        #endregion

        #region Nodes

        /// <summary>
        /// Expression tree node representation
        /// </summary>
        internal abstract class Node
        {
            /// <summary>
            /// Evaluate node (recursively all sub-nodes).
            /// </summary>
            /// <param name="evaluator">Special evaluator strcture for term evaluation</param>
            /// <returns>Result from evaluation of the entire top-down descent tree from the current node.</returns>
            public abstract bool Evaluate(BooleanModel.DocumentTermEvaluator evaluator);
        }

        /// <summary>
        /// Node with having only 1 operand.
        /// </summary>
        private abstract class UnaryNode : Node
        {
            private readonly Node _expression;

            public Node Expression => _expression;

            protected UnaryNode(Node expression)
            {
                _expression = expression;
            }
        }

        /// <summary>
        /// Node with having 2 operands.
        /// </summary>
        private abstract class BinaryNode : Node
        {
            private readonly Node _leftExpression;
            private readonly Node _rightExpression;

            public Node LeftExpression => _leftExpression;
            public Node RightExpression => _rightExpression;

            protected BinaryNode(Node leftExpression, Node rightExpression)
            {
                _leftExpression = leftExpression;
                _rightExpression = rightExpression;
            }
        }

        /// <summary>
        /// Logical 'AND', node representation
        /// </summary>
        private sealed class AndNode : BinaryNode
        {
            public AndNode(Node leftExpression, Node rightExpression)
                : base(leftExpression, rightExpression)
            {
            }

            /// <inheritdoc/>
            public override bool Evaluate(BooleanModel.DocumentTermEvaluator evaluator)
            {
                return LeftExpression.Evaluate(evaluator) && RightExpression.Evaluate(evaluator);
            }
        }

        /// <summary>
        /// Logical 'OR', node representation
        /// </summary>
        private sealed class OrNode : BinaryNode
        {
            public OrNode(Node leftExpression, Node rightExpression)
                : base(leftExpression, rightExpression)
            {
            }

            /// <inheritdoc/>
            public override bool Evaluate(BooleanModel.DocumentTermEvaluator evaluator)
            {
                return LeftExpression.Evaluate(evaluator) || RightExpression.Evaluate(evaluator);
            }
        }

        /// <summary>
        /// Logical 'NOT', node representation
        /// </summary>
        private sealed class NotNode : UnaryNode
        {
            public NotNode(Node expression)
                : base(expression)
            {
            }

            /// <inheritdoc/>
            public override bool Evaluate(BooleanModel.DocumentTermEvaluator evaluator)
            {
                return !Expression.Evaluate(evaluator);
            }
        }

        /// <summary>
        /// Node representing term value (user searched values/text in the query)
        /// </summary>
        private sealed class TermNode : Node
        {
            private readonly string _term;

            public string Term => _term;

            public TermNode(string term)
            {
                _term = term;
            }

            /// <inheritdoc/>
            public override bool Evaluate(BooleanModel.DocumentTermEvaluator evaluator)
            {
                return evaluator.EvaluateTerm(Term);
            }
        }

        #endregion
    }
}

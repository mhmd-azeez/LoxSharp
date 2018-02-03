using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxSharp
{
    public class Interpreter : IVisitor<object>
    {
        public void Interpret(Expr expression)
        {
            try
            {
                var value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch(RuntimeException ex)
            {
                Lox.RuntimeError(ex);
            }
        }

        private string Stringify(object value)
        {
            if (value is null) return "nil";

            if (value is double)
            {
                var text = value.ToString();

                // if the number was an inteter, print it without a decimal point
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return value.ToString();
        }

        public object VisitBinaryExpr(Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch(expr.Operator.Type)
            {
                case TokenType.Greater:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;

                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;

                case TokenType.Less:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;

                case TokenType.LessEqual:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                case TokenType.Minus:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;

                case TokenType.Slash:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;

                case TokenType.Star:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;

                case TokenType.EqualEqual:
                    return IsEqual(left, right);

                case TokenType.BangEqual:
                    return !IsEqual(left, right);

                case TokenType.Plus:
                    if (left is string t1 && right is string t2)
                    {
                        return t1 + t2;
                    }
                    else if (left is double n1 && right is double n2)
                    {
                        return n1 + n2;
                    }

                    throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.");
            }

            // Unreachable
            return null;
        }

        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;

            throw new RuntimeException(@operator, "Operands must be numbers.");
        }

        private bool IsEqual(object left, object right)
        {
            // nil is only equal to nil
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Unary expr)
        {
            var right = Evaluate(expr.Right);

            switch(expr.Operator.Type)
            {
                case TokenType.Minus:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
                case TokenType.Bang:
                    return !IsTruthy(right);
            }

            return null;
        }

        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is double) return;

            throw new RuntimeException(@operator, "Operand must be a number.");
        }

        private bool IsTruthy(object value)
        {
            // TODO: 0 is is also falsy?
            if (value == null) return false;
            if (value is bool b) return b;

            return true;
        }

        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }
    }
}

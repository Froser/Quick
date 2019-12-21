using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick.Plugins.Calc
{
    internal class QuickCalcParse
    {
        public QuickCalcParse()
        {
        }

        public void Add(QuickCalcToken t, string i)
        {
            m_tokenList.Add(t);
            m_symbolList.Add(i);
        }

        public void ClearAll()
        {
            m_tokenList.Clear();
            m_symbolList.Clear();
        }

        public QuickCalcToken GetToken(int index)
        {
            return m_tokenList[index];
        }

        public string GetValue(int index)
        {
            return m_symbolList[index];
        }

        public double Parse()
        {
            m_parseProgress = 0;
            return Stmt_legalexp();
        }

        private void Match(string expected)
        {
            if (m_symbolList[m_parseProgress] == expected)
            {
                m_parseProgress++;
            }
            else
            {
                QuickCalcLog.Log("");
                throw new QuickCalcException("不匹配的标记");
            }
        }

        private double Stmt_legalexp()
        {
            return Stmt_assign();
        }

        private double Stmt_assign()
        {
            return Stmt_expcmp();
        }

        private double Stmt_expcmp()
        {
            double result = Stmt_termcmp();
            bool rA = true;
            bool rB = true;
            if ((int)result == 0)
                rA = false;
            else
                rA = true;

            while (m_parseProgress < m_symbolList.Count() && m_symbolList[m_parseProgress] == ("&&"))
            {
                Match("&&");
                if ((int)Stmt_termcmp() != 0)
                {
                    rB = true;
                }
                else
                    rB = false;
                if (rA && rB) 
                    return 1;
                return 0;
            }

            return result;
        }

        private double Stmt_termcmp()
        {
            double result = Stmt_factorcmp();
            bool rA = false;
            bool rB = false;
            if ((int)result == 0)
                rA = false;
            else
                rA = true;

            while (m_parseProgress < m_symbolList.Count() && m_symbolList[m_parseProgress] == "||")
            {
                Match("||");
                if ((int)Stmt_factorcmp() != 0)
                {
                    rB = true;
                }
                else
                    rB = false;

                if (rA || rB)
                    return 1; 
                return 0;
            }

            return result;
        }

        private double Stmt_factorcmp()
        {
            double result = Stmt_exp();
            while (m_parseProgress < m_symbolList.Count() && 
                    (m_symbolList[m_parseProgress] == ">" ||
                    m_symbolList[m_parseProgress] == "<" ||
                    m_symbolList[m_parseProgress] == "==" ||
                    m_symbolList[m_parseProgress] == ">=" ||
                    m_symbolList[m_parseProgress] == "<=" ||
                    m_symbolList[m_parseProgress] == "!="))
            {
                if (m_symbolList[m_parseProgress] == ">")
                {
                    Match(">");
                    if (result > Stmt_exp()) result = 1; else result = 0;
                }
                else if (m_symbolList[m_parseProgress] == "<")
                {
                    Match("<");
                    if (result < Stmt_exp()) result = 1; else result = 0;
                }
                else if (m_symbolList[m_parseProgress] == "==")
                {
                    Match("==");
                    if (result == Stmt_exp()) result = 1; else result = 0;
                }
                else if (m_symbolList[m_parseProgress] == ">=")
                {
                    Match(">=");
                    if (result >= Stmt_exp()) result = 1; else result = 0;
                }
                else if (m_symbolList[m_parseProgress] == "<=")
                {
                    Match("<=");
                    if (result <= Stmt_exp()) result = 1; else result = 0;
                }
                else if (m_symbolList[m_parseProgress] == "!=")
                {
                    Match("!=");
                    if (result != Stmt_exp()) result = 1; else result = 0;
                }
            }
            return result;
        }

        private double Stmt_exp()
        {
            double result = Stmt_term();
            while (m_parseProgress < m_symbolList.Count() && 
                (m_symbolList[m_parseProgress] == "+" || m_symbolList[m_parseProgress] == "-"))
            {
                if (m_symbolList[m_parseProgress] == "+")
                {
                    Match("+");
                    result += Stmt_term();
                }
                else if (m_symbolList[m_parseProgress] == "-")
                {
                    Match("-");
                    result -= Stmt_term();
                }
            }
            return result;
        }

        private double Stmt_term()
        {
            double result = Stmt_power();
            while (m_parseProgress < m_symbolList.Count() && 
                (m_symbolList[m_parseProgress] == "*" || m_symbolList[m_parseProgress] == "/"))
            {
                if (m_symbolList[m_parseProgress] == "*")
                {
                    Match("*");
                    result *= Stmt_power();
                }
                else if (m_symbolList[m_parseProgress] == "/")
                {
                    Match("/");
                    result /= Stmt_power();
                }
            }

            return result;
        }

        private double Stmt_power()
        {
            double _result;
            _result = Stmt_definedfunction();
            while (m_parseProgress < m_symbolList.Count() && m_symbolList[m_parseProgress] == "^")
            {
                if (m_symbolList[m_parseProgress] == "^")
                {
                    Match("^");
                    _result = Math.Pow(_result, Stmt_definedfunction());
                }
            }

            return _result;
        }

        private double Stmt_definedfunction()
        {
            double result;
            if (m_symbolList[m_parseProgress] == "sin")
            {
                Match("sin");
                Match("(");
                result = Math.Sin(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "cos")
            {
                Match("cos");
                Match("(");
                result = Math.Cos(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "tan")
            {
                Match("tan");
                Match("(");
                result = Math.Tan(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "asin")
            {
                Match("asin");
                Match("(");
                result = Math.Asin(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "acos")
            {
                Match("acos");
                Match("(");
                result = Math.Acos(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "atan")
            {
                Match("atan");
                Match("(");
                result = Math.Atan(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "ln")
            {
                Match("ln");
                Match("(");
                result = Math.Log(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "sqrt")
            {
                Match("sqrt");
                Match("(");
                result = Math.Sqrt(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "abs")
            {
                Match("abs");
                Match("(");
                result = Math.Abs(Stmt_expcmp());
                Match(")");
            }
            else if (m_symbolList[m_parseProgress] == "int")
            {
                Match("int");
                Match("(");
                result = (int)(Stmt_expcmp());
                Match(")");
            }
            else
            {
                result = Stmt_factor();
            }
            return result;
        }

        private double Stmt_factor()
        {
            double result;
            if (m_tokenList[m_parseProgress] == QuickCalcToken.Numeric)
            {
                string symbol = m_symbolList[m_parseProgress];
                bool isHex = symbol.ToLower().StartsWith("0x");
                if (isHex)
                    result = Convert.ToInt16(symbol, 16);
                else
                    result = Double.Parse(symbol);
                m_parseProgress++;
            }
            else if (m_symbolList[m_parseProgress] == "-")
            {
                m_parseProgress++;
                result = -Stmt_factor();
            }
            else if (m_symbolList[m_parseProgress] == "+")
            {
                m_parseProgress++;
                result = Stmt_factor();
            }
            else if (m_symbolList[m_parseProgress] == "(")
            {
                Match("(");
                result = Stmt_expcmp();
                Match(")");
            }
            else
            {
                QuickCalcLog.Log("");
                throw new QuickCalcException("找不到标识符");
            }
            return result;
        }

        private List<QuickCalcToken> m_tokenList = new List<QuickCalcToken>();
        private List<string> m_symbolList = new List<string>();
        private int m_parseProgress;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Froser.Quick.Plugins.Calc
{
    internal class QuickCalcScanner
    {
        private enum DFAState { Start, InId, InInt, InDecimal, InHex, InSym, InCmp, Error }
        private static readonly string[] KEYWORDS = new string[] { "sin", "cos", "tan", "asin", "acos", "atan", "sqrt", "ln", "abs" };
        private static readonly char[] ALL_AVAILABLE_OP = new char[] { '+', '-', '*', '/', '(', ')', '=', '^', ',', '>', '<', '!', '&', '|' };
        private static readonly char[] ALL_COMPARE_OP = { '>', '<', '=', '!', '&', '|' };

        public QuickCalcScanner()
        {
        }

        public double Eval(string line)
        {
            QuickCalcParse parser = Scan(line);
            return parser.Parse();
        }

        private QuickCalcParse Scan(string line)
        {
            QuickCalcParse parser = new QuickCalcParse();
            StringBuilder result = new StringBuilder();
            int index = 0;  //获取形参s的每一个字符的索引

            DFAState currentState = DFAState.Start;    //当前DFA的状态

            while (index < line.Length)
            {    //DFA循环
                switch (currentState)
                {
                    case DFAState.Start:
                        if (char.IsWhiteSpace(line[index]))
                        {
                        }
                        else if (char.IsLetter(line[index]))
                        {
                            result.Append (line[index]);
                            currentState = DFAState.InId;
                        } 
                        else if (char.IsDigit(line[index]))
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InInt;
                        }
                        else if (line[index] == '.')
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InDecimal;
                        }
                        else if (ALL_AVAILABLE_OP.Contains(line[index]))
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InSym;
                        }
                        else
                        {
                            QuickCalcLog.Log("");
                            parser.Add(QuickCalcToken.Error, line);
                        }
                        index++;
                        break;
                    case DFAState.InInt:
                        if (line[index] == '.')
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InDecimal;
                        }
                        else if (result[0] == '0' && (line[index] == 'x' || line[index] == 'X'))
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InHex;
                        }
                        else if (!char.IsDigit(line[index]))
                        {
                            parser.Add(QuickCalcToken.Numeric, result.ToString());    //写入语法分析表
                            currentState = DFAState.Start;
                            result.Clear();    //清空字符串
                            index--;    //回退
                        }
                        else if (char.IsDigit(line[index]))
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InInt;
                        }
                        index++;
                        break;
                    case DFAState.InDecimal:
                        if (!char.IsDigit(line[index]))
                        {
                            parser.Add(QuickCalcToken.Numeric, result.ToString());    //写入语法分析表
                            currentState = DFAState.Start;
                            result.Clear();    //清空字符串
                            index--;    //回退
                        }
                        else if (char.IsDigit(line[index]))
                        {
                            result.Append(line[index]);
                        }
                        index++;
                        break;
                    case DFAState.InHex:
                        if (char.IsDigit(line[index]) || 
                            (line[index] >= 'a' && line[index] <= 'f') ||
                            (line[index] >= 'A' && line[index] <= 'F'))
                        {
                            result.Append(line[index]);
                        }
                        else if (!char.IsDigit(line[index]))
                        {
                            parser.Add(QuickCalcToken.Numeric, result.ToString());    //写入语法分析表
                            currentState = DFAState.Start;
                            result.Clear();    //清空字符串
                            index--;    //回退
                        }
                        index++;
                        break;
                    case DFAState.InId:
                        if (!char.IsLetter(line[index]))
                        {
                            if (isReserved(result.ToString()))    //判断是否为关键字
                            {
                                parser.Add(QuickCalcToken.Reserved, result.ToString());
                                currentState = DFAState.Start;
                                result.Clear();
                                index--;
                            }
                            else
                            {
                                QuickCalcLog.Log("");
                                parser.Add(QuickCalcToken.Error, line);
                            }
                        }
                        else if (char.IsLetter(line[index]))
                        {
                            result.Append(line[index]);
                            currentState = DFAState.InId;
                        }
                        index++;
                        break;
                    case DFAState.InSym:
                        index--;
                        if (ALL_COMPARE_OP.Contains(line[index]))
                        {
                            currentState = DFAState.InCmp;
                        }
                        else
                        {
                            parser.Add(QuickCalcToken.Symbol, result.ToString());
                            currentState = DFAState.Start;
                            result.Clear();
                        }
                        index++;
                        break;
                    case DFAState.InCmp:
                        switch (line[index - 1])
                        {    //判断前一个字符是<,>,=还是!，他们只能后接=符号，否则将作为两个符号处理
                            case '>':
                            case '<':
                            case '=':
                            case '!':
                                if (line[index] == '=')
                                {
                                    result.Append(line[index]);
                                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                                }
                                else
                                {    //作为两个符号保存
                                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                                    index--;
                                }
                                index++;
                                currentState = DFAState.Start;
                                break;
                            case '&':
                                if (line[index] == '&')
                                {
                                    result.Append(line[index]);
                                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                                    index++;
                                }
                                else
                                {    //作为两个符号保存
                                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                                }
                                currentState = DFAState.Start;
                                result.Clear();
                                break;
                            case '|':
                                if (line[index] == '|')
                                {
                                    result.Append(line[index]);
                                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                                    index++;
                                }
                                else
                                {    //作为两个符号保存
                                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                                }
                                currentState = DFAState.Start;
                                result.Clear();
                                break;
                        }
                        break;
                }
            }

            //处理字符串尾端的状态
            switch (currentState)
            {
                case DFAState.InInt:
                case DFAState.InDecimal:
                case DFAState.InHex:
                    parser.Add(QuickCalcToken.Numeric, result.ToString());    //写入语法分析表
                    break;
                case DFAState.InId:
                    if (isReserved(result.ToString()))
                    {    //判断是否为关键字
                        parser.Add(QuickCalcToken.Reserved, result.ToString());
                    }
                    else
                    {
                        parser.Add(QuickCalcToken.Error, result.ToString());
                    }
                    break;
                case DFAState.InSym:
                    parser.Add(QuickCalcToken.Symbol, result.ToString());
                    break;
            }

            return parser;
        }

        private bool isReserved(String s)
        {    //判断是否为关键字
            return KEYWORDS.Contains(s.ToLower());
        }
    }
}

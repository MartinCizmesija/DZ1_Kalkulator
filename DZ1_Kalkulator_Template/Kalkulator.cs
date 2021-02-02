using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace PrvaDomacaZadaca_Kalkulator
{
    public class Factory
    {
        public static ICalculator CreateCalculator()
        {
            // vratiti kalkulator
            Thread.CurrentThread.CurrentCulture = new CultureInfo("hr-HR");
            return new Kalkulator();
        }
    }

    public class Kalkulator:ICalculator
    {
        bool leadingZero = false;
        bool rounded = false;
        bool wasLastOperand = false;
        char operand = ' ';
        double lastOperand = 0;
        double currentOperand = 0;
        List<char> state = new List<char> { };
        List<char> memory = new List<char> { };

        public void Press(char inPressedDigit)
        {
            if (inPressedDigit.Equals('P'))
            {
                if (state.Count == 0) return;

                else
                {
                    memory.Clear();
                    if (leadingZero) memory.Add('-');
                    for (int i = 0; i < state.Count; ++i)
                    {
                        memory.Add(state[i]);
                    }
                    return;
                }
            }

            if (inPressedDigit.Equals('G'))
            {
                state.Clear();
                state = memory;
                return;
            }

            if (inPressedDigit.Equals('O'))
            {
                leadingZero = false;
                rounded = false;
                wasLastOperand = false;
                operand = ' ';
                lastOperand = 0;
                currentOperand = 0;
                state.Clear();
                memory.Clear();
                return;
            }

            if (inPressedDigit.Equals('C'))
            {
                leadingZero = false;
                rounded = false;
                wasLastOperand = false;
                state.Clear();
                return;
            }

            if (inPressedDigit.Equals('M')) 
            {
                leadingZero = !leadingZero;
                return;
            }

            if (inPressedDigit.Equals(','))
            {
                if (state.Count == 0)
                {
                    wasLastOperand = false;
                    state.Add('0');
                    state.Add(inPressedDigit);
                    return;
                }

                state.Add(inPressedDigit);
                return;
            }

            if (inPressedDigit.Equals('I'))
            {
                if (state.Count == 0 && currentOperand == 0)
                {
                    throwExc();
                    return;
                }

                if (state.Count != 0)
                {
                    currentOperand = Convert.ToDouble(string.Join("", state));
                }

                currentOperand = 1 / currentOperand;

                if (currentOperand > 9999999999)
                {
                    throwExc();
                    return;
                }

                fillState(currentOperand);
                return;
            }

            if (inPressedDigit.Equals('Q'))
            {
                if (state.Count != 0)
                {
                    currentOperand = Convert.ToDouble(string.Join("", state));
                }
                currentOperand = currentOperand * currentOperand;

                if (currentOperand > 9999999999)
                {
                    throwExc();
                    return;
                }

                fillState(currentOperand);
                return;
            }

            if (inPressedDigit.Equals('R'))
            {
                if (state.Count != 0)
                {
                    currentOperand = Convert.ToDouble(string.Join("", state));
                }
                currentOperand = Math.Sqrt(currentOperand);

                if (currentOperand > 9999999999)
                {
                    throwExc();
                    return;
                }

                fillState(currentOperand);
                return;
            }

            //sinus
            if (inPressedDigit.Equals('S'))
            {
                if (state.Count == 0) currentOperand = Math.Sin(currentOperand);

                if (leadingZero)
                {
                    currentOperand = Math.Sin(Convert.ToDouble("-" + string.Join("", state)));
                }
                else
                {
                    currentOperand = Math.Sin(Convert.ToDouble(string.Join("", state)));
                }

                fillState(currentOperand);
                return;
            }

            //kosinus
            if (inPressedDigit.Equals('K'))
            {
                if (state.Count == 0) currentOperand = Math.Sin(currentOperand);

                if (leadingZero)
                {
                    currentOperand = Math.Cos(Convert.ToDouble("-" + string.Join("", state)));
                }
                else
                {
                    currentOperand = Math.Cos(Convert.ToDouble(string.Join("", state)));
                }

                fillState(currentOperand);
                return;
            }

            //tangens
            if (inPressedDigit.Equals('T'))
            {
                if (state.Count == 0) currentOperand = Math.Sin(currentOperand);

                if (leadingZero)
                {
                    currentOperand = Math.Tan(Convert.ToDouble("-" + string.Join("", state)));
                }
                else
                {
                    currentOperand = Math.Tan(Convert.ToDouble(string.Join("", state)));
                }

                fillState(currentOperand);
                return;
            }

            if (inPressedDigit.Equals('='))
            {
                if (state.Count == 0)
                {
                    if (operand.Equals(' ')) return;
                    operate();
                    fillState(lastOperand);
                    return;
                }

                if (leadingZero)
                {
                    currentOperand = Convert.ToDouble("-" + string.Join("", state));
                }
                else currentOperand = Convert.ToDouble(string.Join("", state));
                
                operate();

                if (lastOperand > 9999999999) {
                    throwExc();
                    return;
                }

                fillState(lastOperand);
                return;
            }

            if (!Char.IsDigit(inPressedDigit))
            {
                if(wasLastOperand)
                {
                    operand = inPressedDigit;
                    return;
                }

                if (leadingZero)
                {
                    currentOperand = Convert.ToDouble("-" + string.Join("", state));
                }
                else currentOperand = Convert.ToDouble(string.Join("", state));

                operate();

                if (lastOperand > 9999999999)
                {
                    throwExc();
                    return;
                }

                operand = inPressedDigit;
                wasLastOperand = true;
                leadingZero = false;
                state.Clear();
                return;
            }

            if (state.Count > 9)
            {
                if (state.Count > 10)
                {
                    if (!rounded && Char.IsDigit(state[state.Count - 1]) && Char.IsDigit(inPressedDigit) && (int)Char.GetNumericValue(inPressedDigit) > 4)
                    {
                        int val = (int)Char.GetNumericValue(state[state.Count - 1]) + 1;
                        state[state.Count - 1] = (char)val;
                        rounded = true;
                        return;
                    }
                    return;
                }

                if (state.Contains(',') && !state[state.Count-1].Equals(','))
                {
                    state.Add(inPressedDigit);
                    return;
                }
                return;
            }

            if (inPressedDigit.Equals('0'))
            {
                if (state.Count != 0)
                {
                    if (state.Contains(',') || !state.First().Equals('0'))
                    {
                        state.Add(inPressedDigit);
                        wasLastOperand = false;
                        return;
                    }
                    return;
                }
            }

            if (Char.IsDigit(inPressedDigit)) 
            {
                if (wasLastOperand)
                {
                    state.Clear();
                }
                wasLastOperand = false;
                state.Add(inPressedDigit);
                return;
            }
            return;
        }

        public string GetCurrentDisplayState()
        {
            if (state.Count == 0) 
            {
                if (lastOperand != 0) return lastOperand.ToString();
                return "0";
            }

            if(leadingZero) 
            {
                return "-" + string.Join("", state);
            } else return string.Join("", state);
        }

        public void throwExc()
        {
            state.Clear();
            state.Add('-');
            state.Add('E');
            state.Add('-');
            return;
        }

        public void fillState(double number)
        {
            List<char> numArray = new List<char> { };
            numArray = number.ToString().ToList();

            state.Clear();
            for (int i = 0; i < numArray.Count; ++i)
            {
                if (numArray[i].Equals('-'))
                {
                    leadingZero = true;
                    continue;
                }

                if (state.Count > 10 && numArray.Contains(','))
                {
                    if (Convert.ToDouble(string.Join("", numArray[i])) > 4 && !numArray[i].Equals(','))
                    {
                        int val = state.Last() + 1;
                        state[state.Count - 1] = (char)val;
                    }
                    return;
                }
                if (state.Count > 11) return;

                state.Add(numArray[i]);
            }
        }

        public void operate()
        {
            if (!operand.Equals(' '))
            {
                if (operand.Equals('+'))
                {
                    lastOperand += currentOperand;
                }
                if (operand.Equals('-'))
                {
                    lastOperand = lastOperand - currentOperand;
                }
                if (operand.Equals('*'))
                {
                    lastOperand = lastOperand * currentOperand;
                }
                if (operand.Equals('/'))
                {
                    lastOperand = lastOperand / currentOperand;
                }

            }
            else lastOperand = currentOperand;
            return;
        }


    } 
}

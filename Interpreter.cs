using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Interpreter
    {
        private string Program_Text;
        private Generator_OPS Generator;
        private List<Generator_OPS.ItemOPS> ItemsOPS;

        private int GetNum(Generator_OPS.ItemOPS itemOPS)
        {
            if (itemOPS.Type_OPS == Generator_OPS.TypeOPS.NUMBER)
                return itemOPS.Number;
            else if (itemOPS.Type_OPS == Generator_OPS.TypeOPS.VARIABLE)
                if ((Generator.GetVariables()).ContainsKey(itemOPS.Variable_Name))
                    return Generator.GetVariables()[itemOPS.Variable_Name];
                else return Generator.GetArrays()[itemOPS.Variable_Name][itemOPS.Array_Index];
            else throw new Exception($"Variable or number was expected, (row='{itemOPS.Row}', col='{itemOPS.Col}')");
        }
        private void SetNum(Generator_OPS.ItemOPS itemOPS, int num)
        {
            if (Generator.GetVariables().ContainsKey(itemOPS.Variable_Name))
                Generator.SetVariable(itemOPS.Variable_Name, num);
            else if (Generator.GetArrays().ContainsKey(itemOPS.Variable_Name))
                Generator.SetArrayValue(itemOPS.Variable_Name, itemOPS.Array_Index, num);
            else throw new Exception($"A variable was expected, (row='{itemOPS.Row}', col='{itemOPS.Col}')");
        }
        
        public Interpreter(string program_text)
        {
            Program_Text = program_text;
            Generator = new Generator_OPS();
        }
        public void RunInterpreter()
        {
            ItemsOPS = Generator.RunGenerate(Analyzer.Analyze_Text(Program_Text));

            Stack<Generator_OPS.ItemOPS> magazine = new Stack<Generator_OPS.ItemOPS>();

            for(int i = 0; i < ItemsOPS.Count; i++)
            {
                switch(ItemsOPS[i].Type_OPS)
                {
                    case Generator_OPS.TypeOPS.VARIABLE:
                        {
                            if (!Generator.GetVariables().ContainsKey(ItemsOPS[i].Variable_Name) &&
                                !Generator.GetArrays().ContainsKey(ItemsOPS[i].Variable_Name))
                                throw new Exception($"Unknown variable '{ItemsOPS[i].Variable_Name}', (row='{ItemsOPS[i].Row}', col='{ItemsOPS[i].Col}')");
                            magazine.Push(ItemsOPS[i]);
                            break;
                        }
                    case Generator_OPS.TypeOPS.NUMBER:
                        {
                            magazine.Push(ItemsOPS[i]);
                            break;
                        }
                    case Generator_OPS.TypeOPS.OPERATION:
                        {
                            switch(ItemsOPS[i].Operation_OPS)
                            {
                                case Generator_OPS.OperationOPS.READ:
                                    {
                                        int num = int.Parse(Console.ReadLine());
                                        Generator_OPS.ItemOPS itemOPS = magazine.Pop();
                                        SetNum(itemOPS, num);
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.WRITE:
                                    {
                                        Generator_OPS.ItemOPS itemOPS = magazine.Pop();
                                        Console.WriteLine(GetNum(itemOPS));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.PLUS:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) + GetNum(itemOPS_2), itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.MINUS:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) - GetNum(itemOPS_2), itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.MULTIPLY:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) * GetNum(itemOPS_2), itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.DIVIDE:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        if (GetNum(itemOPS_2) == 0)
                                            throw new Exception($"Division by zero, (row='{ItemsOPS[i].Row}', col='{ItemsOPS[i].Col}')");
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) / GetNum(itemOPS_2), itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.LESS:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) < GetNum(itemOPS_2) ? 1 : 0, itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.MORE:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) > GetNum(itemOPS_2) ? 1 : 0, itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.EQUAL:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) == GetNum(itemOPS_2) ? 1 : 0, itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.LESS_OR_EQUAL:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) <= GetNum(itemOPS_2) ? 1 : 0, itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.MORE_OR_EQUAL:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) >= GetNum(itemOPS_2) ? 1 : 0, itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.NOT_EQUAL:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        magazine.Push(new Generator_OPS.ItemOPS(GetNum(itemOPS_1) != GetNum(itemOPS_2) ? 1 : 0, itemOPS_1.Row, itemOPS_1.Col));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.ASSIGN:
                                    {
                                        Generator_OPS.ItemOPS itemOPS_2 = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS_1 = magazine.Pop();
                                        SetNum(itemOPS_1, GetNum(itemOPS_2));
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.J:
                                    {
                                        i = GetNum(magazine.Pop()) - 1;
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.JF:
                                    {
                                        Generator_OPS.ItemOPS metka = magazine.Pop();
                                        Generator_OPS.ItemOPS itemOPS = magazine.Pop();
                                        if(GetNum(itemOPS) == 0)
                                        {
                                            i = GetNum(metka) - 1;
                                        }
                                        break;
                                    }
                                case Generator_OPS.OperationOPS.I:
                                    {
                                        Generator_OPS.ItemOPS itemIDX = magazine.Pop();
                                        Generator_OPS.ItemOPS array = magazine.Pop();
                                        array.Array_Index = GetNum(itemIDX);
                                        magazine.Push(array);
                                        break;
                                    }
                                default:
                                    throw new Exception($"Unknown operation ops, (row='{ItemsOPS[i].Row}', col='{ItemsOPS[i].Col}')");
                            }
                            break;
                        }
                }
            }
        }
    }
}

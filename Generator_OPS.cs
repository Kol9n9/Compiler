using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Generator_OPS
    {
        public enum TypeOPS
        {
            VARIABLE,
            NUMBER,
            OPERATION,
            ERROR
        }
        public enum OperationOPS
        {
            READ, 
            WRITE, 
            PLUS, 
            MINUS, 
            MULTIPLY,
            DIVIDE, 
            LESS,
            ASSIGN, // =
            MORE, 
            EQUAL, // ==
            LESS_OR_EQUAL,
            MORE_OR_EQUAL, 
            NOT_EQUAL, // !=
            J, // just jump
            JF, // jump if false
            I, // index
            ERROR
        }

        public enum OPSMachineState
        {
            P, // → intIP | arrayAP | aH = E; Q | read(aH); Q | write(E); Q | if (C) { SQ }KZQ | while (C) { SQ }Q
            I, // → aM
            M, // → ,aM | ;
            A, // → a[k]N
            N, // → ,a[k]N | ;
            Q, // → aH = E; Q | read(aH); Q | write(E); Q | if (C) { SQ }KZQ | while (C) { SQ }Q | λ
            S, // → aH = E; | read(aH); | write(E); | if (C) { SQ }KZ | while (C) { SQ }
            H, // → [E] | λ
            C, // → (E)VUD | aHVUD | kVUD
            D, // → < EZ | > EZ | == EZ | ≤ EZ | ≥ EZ | != EZ
            K, // → else { SQ } | λ
            E, // → (E)VU | aHVU | kVU
            U, // → + TU | -TU | λ
            T, // → (E)V | aHV | kV
            V, // → *FV | /FV | λ
            F, // → (E) | aH | k
            Z, // → λ
            ERROR // error state
        }
        public enum GenTask
        {
            EMPTY, // пустое действие
            VARIABLE, // записать идентификатор переменной
            NUM, // записать целое число без знака
            READ, // записать оператор чтения из потока
            WRITE, // записать оператор записи в поток
            PLUS, // записать +
            MINUS, // записать -
            MULTIPLY, // записать *
            DIVIDE, // записать /
            LESS, // записать <
            ASSIGN, // записать =
            MORE, // записать >
            EQUAL, // записать ==
            LESS_OR_EQUAL, // записать <=
            MORE_OR_EQUAL, // записать >=
            NOT_EQUAL, // записать !=
            I, // индекс
            TASK1, // при IF и WHILE
            TASK2, // при ELSE
            TASK3, // при IF
            TASK4, // при WHILE
            TASK5, // при WHILE
            TASK6, // при INT
            TASK7, // при ARRAY
            TASK8, // при VARIABLES
            TASK9, // при VARIABLES
        }
        public class MagazineItem
        {
            public bool IsTerminal { get; set; }
            public Analyzer.LexemeType Lexeme_Type { get; set; }
            public OPSMachineState MachineState { get; set; }
            public MagazineItem(Analyzer.LexemeType lexemetype)
            {
                IsTerminal = true;
                Lexeme_Type = lexemetype;
                MachineState = OPSMachineState.ERROR;
            }
            public MagazineItem(OPSMachineState machineState)
            {
                IsTerminal = false;
                Lexeme_Type = Analyzer.LexemeType.ERROR;
                MachineState = machineState;
            }
        }
        public class ItemOPS
        {
            public TypeOPS Type_OPS { get; set; }
            public OperationOPS Operation_OPS { get; set; }
            public string Variable_Name { get; set; }
            public int Array_Index { get; set; }
            public int Number { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
            public ItemOPS(string name, Analyzer.Lexeme lexeme)
            {
                Variable_Name = name;
                Type_OPS = TypeOPS.VARIABLE;
                Operation_OPS = OperationOPS.ERROR;
                Row = lexeme.Row;
                Col = lexeme.Col;
            }
            public ItemOPS(OperationOPS operation, Analyzer.Lexeme lexeme)
            {
                Type_OPS = TypeOPS.OPERATION;
                Operation_OPS = operation;
                Row = lexeme.Row;
                Col = lexeme.Col;
            }
            public ItemOPS(int num, Analyzer.Lexeme lexeme)
            {
                Type_OPS = TypeOPS.NUMBER;
                Operation_OPS = OperationOPS.ERROR;
                Number = num;
                Row = lexeme.Row;
                Col = lexeme.Col;
            }
            public ItemOPS(int num, int row, int col)
            {
                Type_OPS = TypeOPS.NUMBER;
                Operation_OPS = OperationOPS.ERROR;
                Number = num;
                Row = row;
                Col = col;
            }
        }
        public enum Table
        {
            VARIABLE,
            ARRAY
        }
        private int aaa = 0;
        void NextState(OPSMachineState machineState, Analyzer.Lexeme lexeme)
        {
            switch(machineState)
            {
                case OPSMachineState.P:
                    { 
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.INT:
                                { 
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.P));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.I));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.INT));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK6);


                                    break;

                                }
                            case Analyzer.LexemeType.ARRAY:
                                { 
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.P));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.A));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.ARRAY));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK7);
                                    break;
                                }
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));

                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.ASSIGN));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.ASSIGN);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);

                                    break;
                                }
                            case Analyzer.LexemeType.READ:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.READ));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.READ);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.WRITE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.WRITE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.WRITE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.IF:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.K));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.C));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.IF));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK3);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK1);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            
                            case Analyzer.LexemeType.WHILE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.C));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.WHILE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK5);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK1);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK4);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}') aaa = {aaa}");  
                        }
                        aaa++;
                        break;
                    }
                case OPSMachineState.I:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.M));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK8);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");
                        }
                        break;
                    }
                case OPSMachineState.M:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.COMMA:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.M));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.COMMA));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK8);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.SEMICOLON:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));

                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            default:
                                throw new Exception($"M Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");
                        }
                        break;
                    }
                case OPSMachineState.A:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.N));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_SQUARE_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NUMBER));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_SQUARE_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK9);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK8);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");

                        }
                        break;
                    }
                case OPSMachineState.N:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.COMMA:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.N));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_SQUARE_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NUMBER));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_SQUARE_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.COMMA));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK9);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK8);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.SEMICOLON:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));

                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");

                        }
                        break;
                    }
                case OPSMachineState.Q:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.ASSIGN));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.ASSIGN);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    break;
                                }
                            case Analyzer.LexemeType.READ:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.READ));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.READ);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.WRITE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.WRITE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.WRITE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.IF:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.K));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.C));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.IF));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK3);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK1);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.WHILE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.C));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.WHILE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK5);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK1);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK4);
                                    break;
                                }
                            default:
                               break;

                        }
                        break;
                    }
                case OPSMachineState.S:
                    {
                        switch (lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.ASSIGN));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.ASSIGN);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    break;
                                }
                            case Analyzer.LexemeType.READ: //////////
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.READ));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.READ);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.WRITE:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.SEMICOLON));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.WRITE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.WRITE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.IF:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.K));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.C));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.IF));

                                    GenTasks.Push(GenTask.TASK3);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK1);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.WHILE:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.C));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.WHILE));

                                    GenTasks.Push(GenTask.TASK5);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK1);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK4);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");
                        }
                        break;
                    }
                case OPSMachineState.H:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.LEFT_SQUARE_BRACKET:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_SQUARE_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_SQUARE_BRACKET));

                                    GenTasks.Push(GenTask.I);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case OPSMachineState.C:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.LEFT_ROUND_BRACKET:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.D));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.D));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    break;
                                }
                            case Analyzer.LexemeType.NUMBER:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.D));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NUMBER));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.NUM);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");
                        }
                        break;
                    }
                case OPSMachineState.D:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.LESS:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LESS));

                                    GenTasks.Push(GenTask.LESS);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.MORE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.MORE));

                                    GenTasks.Push(GenTask.MORE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.EQUAL:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.EQUAL));

                                    GenTasks.Push(GenTask.EQUAL);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.LESS_OR_EQUAL:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LESS_OR_EQUAL));

                                    GenTasks.Push(GenTask.LESS_OR_EQUAL);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.MORE_OR_EQUAL:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.MORE_OR_EQUAL));

                                    GenTasks.Push(GenTask.MORE_OR_EQUAL);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.NOT_EQUAL:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Z));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NOT_EQUAL));

                                    GenTasks.Push(GenTask.NOT_EQUAL);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");
                        }
                        break;
                    }
                case OPSMachineState.K:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.ELSE:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_BRACE));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.Q));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.S));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_BRACE));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.ELSE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.TASK2);
                                    break;
                                }
                            default:
                                break;    
                        }
                        break;
                    }
                case OPSMachineState.E:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.LEFT_ROUND_BRACKET:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    break;
                                }
                            case Analyzer.LexemeType.NUMBER:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NUMBER));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.NUM);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");

                        }
                        break;
                    }
                case OPSMachineState.U:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.PLUS:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.T));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.PLUS));

                                    GenTasks.Push(GenTask.PLUS);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.MINUS:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.U));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.T));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.MINUS));

                                    GenTasks.Push(GenTask.MINUS);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case OPSMachineState.T:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.LEFT_ROUND_BRACKET:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    break;
                                }
                            case Analyzer.LexemeType.NUMBER:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NUMBER));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.NUM);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");

                        }
                        break;
                    }
                case OPSMachineState.V:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.MULTIPLY:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.F));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.MULTIPLY));

                                    GenTasks.Push(GenTask.MULTIPLY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.DIVIDE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.F));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.V));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.DIVIDE));

                                    GenTasks.Push(GenTask.DIVIDE);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    }
                case OPSMachineState.F:
                    {
                        switch(lexeme.Lexeme_Type)
                        {
                            case Analyzer.LexemeType.LEFT_ROUND_BRACKET:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.RIGHT_ROUND_BRACKET));
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.E));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.LEFT_ROUND_BRACKET));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.EMPTY);
                                    break;
                                }
                            case Analyzer.LexemeType.VARIABLE:
                                {
                                    GenMagazine.Push(new MagazineItem(OPSMachineState.H));
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.VARIABLE));

                                    GenTasks.Push(GenTask.EMPTY);
                                    GenTasks.Push(GenTask.VARIABLE);
                                    break;
                                }
                            case Analyzer.LexemeType.NUMBER:
                                {
                                    GenMagazine.Push(new MagazineItem(Analyzer.LexemeType.NUMBER));

                                    GenTasks.Push(GenTask.NUM);
                                    break;
                                }
                            default:
                                throw new Exception($"Unexpected lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");

                        }
                        break;
                    }
                case OPSMachineState.Z:
                         break;
                case OPSMachineState.ERROR:
                        throw new Exception($"ERROR lexeme type, (row='{lexeme.Row}', col='{lexeme.Col}')");
            }
        }
        void GenerateTask(GenTask genTask, Analyzer.Lexeme lexeme)
        {
            switch(genTask)
            {
                case GenTask.EMPTY:
                    break;
                case GenTask.VARIABLE:
                    ItemsOPS.Add(new ItemOPS(lexeme.Value, lexeme));
                    break;
                case GenTask.NUM:
                    ItemsOPS.Add(new ItemOPS(int.Parse(lexeme.Value), lexeme));
                    break;
                case GenTask.READ:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.READ, lexeme));
                    break;
                case GenTask.WRITE:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.WRITE, lexeme));
                    break;
                case GenTask.PLUS:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.PLUS, lexeme));
                    break;
                case GenTask.MINUS:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.MINUS, lexeme));
                    break;
                case GenTask.MULTIPLY:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.MULTIPLY, lexeme));
                    break;
                case GenTask.DIVIDE:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.DIVIDE, lexeme));
                    break;
                case GenTask.LESS:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.LESS, lexeme));
                    break;
                case GenTask.ASSIGN:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.ASSIGN, lexeme));
                    break;
                case GenTask.MORE:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.MORE, lexeme));
                    break;
                case GenTask.EQUAL:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.EQUAL, lexeme));
                    break;
                case GenTask.LESS_OR_EQUAL:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.LESS_OR_EQUAL, lexeme));
                    break;
                case GenTask.MORE_OR_EQUAL:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.MORE_OR_EQUAL, lexeme));
                    break;
                case GenTask.NOT_EQUAL:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.NOT_EQUAL, lexeme));
                    break;
                case GenTask.I:
                    ItemsOPS.Add(new ItemOPS(OperationOPS.I, lexeme));
                    break;
                case GenTask.TASK1: // Для условия IF
                    {
                        GenMarks.Push(ItemsOPS.Count);
                        ItemsOPS.Add(new ItemOPS(0, lexeme));
                        ItemsOPS.Add(new ItemOPS(OperationOPS.JF, lexeme));
                        break;
                    }

                case GenTask.TASK2: // Для условия ELSE
                    {
                        int place = GenMarks.Pop();
                        ItemsOPS[place].Number = ItemsOPS.Count + 2;
                        GenMarks.Push(ItemsOPS.Count);
                        ItemsOPS.Add(new ItemOPS(0, lexeme));
                        ItemsOPS.Add(new ItemOPS(OperationOPS.J, lexeme));
                        break;
                    }
                case GenTask.TASK3: // Для продолжения после IF
                    {
                        int place = GenMarks.Pop();
                        ItemsOPS[place].Number = ItemsOPS.Count;
                        break;
                    }
                case GenTask.TASK4: // Для WHILE
                    {
                        GenMarks.Push(ItemsOPS.Count);
                        break;
                    }
                case GenTask.TASK5: // для продолжения WHILE
                    {
                        int place = GenMarks.Pop();
                        ItemsOPS[place].Number = ItemsOPS.Count + 2;
                        ItemsOPS.Add(new ItemOPS(GenMarks.Pop(),lexeme));
                        ItemsOPS.Add(new ItemOPS(OperationOPS.J, lexeme));
                        break;
                    }
                case GenTask.TASK6: // для переменных
                    {
                        CurrentTable = Table.VARIABLE;
                        break;
                    }
                case GenTask.TASK7: // для массивов
                    {
                        CurrentTable = Table.ARRAY;
                        break;
                    }
                case GenTask.TASK8: // добавление имени переменной в текущюю таблицу
                    {
                        string name = lexeme.Value;
                        if(Arrays.ContainsKey(name) || Variables.ContainsKey(name))
                        {
                            throw new Exception($"Redefining a variable '{name}', (row='{lexeme.Row}', col='{lexeme.Col}')");
                        }
                        if(CurrentTable == Table.VARIABLE)
                        {
                            Variables.Add(name, 0);
                        }
                        else
                        {
                            CurrentArrayName = name;
                        }
                        break;
                    }
                case GenTask.TASK9: // Выделение памяти для массивы
                    {
                        int length = int.Parse(lexeme.Value);
                        Arrays.Add(CurrentArrayName, new int[length]);
                        break;
                    }
            }
        }
        public List<ItemOPS> RunGenerate(List<Analyzer.Lexeme> lexemes)
        {
            GenMagazine.Push(new MagazineItem(OPSMachineState.P));
            GenTasks.Push(GenTask.EMPTY);
            CurrentTable = Table.VARIABLE;
            int CurrentLexemeID = 0;
            Analyzer.Lexeme CurrentLexeme = lexemes[0];
            while(GenMagazine.Any() && GenTasks.Any())
            {
                GenerateTask(GenTasks.Pop(), CurrentLexeme);

                MagazineItem CurrentMagazineItem = GenMagazine.Pop();
                if(CurrentMagazineItem.IsTerminal)
                {
                    if (CurrentLexeme.Lexeme_Type == Analyzer.LexemeType.FINISH)
                    {
                        throw new Exception("All lexemes are read, BUT magazine is not empty");
                    }

                    if(CurrentLexeme.Lexeme_Type == CurrentMagazineItem.Lexeme_Type)
                    {
                        CurrentLexemeID++;
                        CurrentLexeme = lexemes[CurrentLexemeID];
                    }
                    else
                    {
                        throw new Exception($"Unexpected lexeme type, (row='{CurrentLexeme.Row}', col='{CurrentLexeme.Col}')");
                    }
                }
                else
                {
                    NextState(CurrentMagazineItem.MachineState, CurrentLexeme);
                }
            }
            return ItemsOPS;
        }
        public Dictionary<string, int> GetVariables()
        {
            return Variables;
        }
        public void SetVariable(string name, int num)
        {
            if (!Variables.ContainsKey(name))
                throw new Exception("A variable was expected");
            Variables[name] = num;
        }
        public Dictionary<string, int[]> GetArrays()
        {
            return Arrays;
        }
        public void SetArrayValue(string name, int index, int num)
        {
            if (!Arrays.ContainsKey(name))
                throw new Exception("A arrays was expected");
            Arrays[name][index] = num;
        }
        
        private Stack<MagazineItem> GenMagazine = new Stack<MagazineItem>(); // Стэк маганиз
        private Stack<GenTask> GenTasks = new Stack<GenTask>(); // Стэк семантические программы
        private List<ItemOPS> ItemsOPS = new List<ItemOPS>(); // Элементы ОПС
        private Stack<int> GenMarks = new Stack<int>(); // Стэк меток
        private Table CurrentTable; // Текущая таблица, куда происходит запись переменных
        private Dictionary<string, int> Variables = new Dictionary<string, int>(); // Записанные переменные
        private Dictionary<string, int[]> Arrays = new Dictionary<string, int[]>(); // Записанные массивы
        private string CurrentArrayName; // Имя массива
    }
}

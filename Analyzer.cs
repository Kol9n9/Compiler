using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    public class Analyzer
    {
        public enum LexemeType
        {
            VARIABLE, // variable
            NUMBER, // number
            INT, // int
            ARRAY, // array
            IF, // if
            ELSE, // else
            WHILE, // while
            READ, // read
            WRITE, // write
            LEFT_BRACE, // {
            RIGHT_BRACE, // }
            LEFT_SQUARE_BRACKET, // [
            RIGHT_SQUARE_BRACKET, // ]
            LEFT_ROUND_BRACKET, // (
            RIGHT_ROUND_BRACKET, // )
            PLUS, // +
            MINUS, // -
            MULTIPLY, // *
            DIVIDE, // /
            SEMICOLON, // ;
            COMMA, // ,
            LESS, // <
            ASSIGN, // =
            MORE, // >
            EQUAL, // ==
            LESS_OR_EQUAL, // <=
            MORE_OR_EQUAL, // >=
            NOT_EQUAL, // !=
            FINISH, // last lexeme
            ERROR // error lexeme 
        }
        public struct Lexeme
        {
            public LexemeType Lexeme_Type { get; set; }
            public string Value { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }

            public Lexeme(LexemeType lexemeType, string value, int row, int col)
            {
                Lexeme_Type = lexemeType;
                Value = value;
                Row = row;
                Col = col;
            }
        }

        public static List<Lexeme> Analyze_Text(string program_text)
        {
            List <Lexeme> Results = new List<Lexeme>();
            int Size = program_text.Length;
            int Current_Row = 1;
            int Current_Col = 1;
            for(int i = 0; i < Size;)
            {
                char Current_Character = program_text[i];
                if(IsSpace(Current_Character))
                {
                    switch(Current_Character)
                    {
                        case ' ':
                            Current_Col++;
                            break;
                        case '\r':
                        case '\n':
                        case '\v':
                        case '\f':
                            Current_Col = 1;
                            Current_Row++;
                            break;
                    }
                    i++;
                }
                else if(IsChar(Current_Character))
                {
                    string value = "";
                    while(IsChar(Current_Character) || IsDigit(Current_Character))
                    {
                        value += Current_Character;
                        i++;
                        if(i >= Size)
                        {
                            break; 
                        }
                        Current_Character = program_text[i];
                    }
                    Results.Add(new Lexeme
                    (
                        value switch
                        {
                            "цел" => LexemeType.INT,
                            "array" => LexemeType.ARRAY,
                            "if" => LexemeType.IF,
                            "else" => LexemeType.ELSE,
                            "while" => LexemeType.WHILE,
                            "read" => LexemeType.READ,
                            "write" => LexemeType.WRITE,
                            _ => LexemeType.VARIABLE
                        }, value, Current_Col, Current_Row
                    ));
                    Current_Col += value.Length;
                }
                else if(IsDigit(Current_Character))
                {
                    string value = "";
                    while(IsDigit(Current_Character))
                    {
                        value += Current_Character;
                        i++;
                        if(i >= Size)
                        {
                            break;
                        }
                        Current_Character = program_text[i];
                    }
                    Results.Add(new Lexeme(LexemeType.NUMBER,value,Current_Row,Current_Col));
                    Current_Col += value.Length;
                }
                else if(Current_Character == '<')
                {
                    if(i + 1 < Size && program_text[i+1] == '=')
                    {
                        Results.Add(new Lexeme(LexemeType.LESS_OR_EQUAL,"<=",Current_Row,Current_Col));
                        i += 2;
                        Current_Col += 2;
                    }
                    else
                    {
                        Results.Add(new Lexeme(LexemeType.LESS, "<", Current_Row, Current_Col));
                        i += 1;
                        Current_Col += 1;
                    }
                }
                else if (Current_Character == '>')
                {
                    if (i + 1 < Size && program_text[i + 1] == '=')
                    {
                        Results.Add(new Lexeme(LexemeType.MORE_OR_EQUAL, ">=", Current_Row, Current_Col));
                        i += 2;
                        Current_Col += 2;
                    }
                    else
                    {
                        Results.Add(new Lexeme(LexemeType.MORE, ">", Current_Row, Current_Col));
                        i += 1;
                        Current_Col += 1;
                    }
                }
                else if (Current_Character == '=')
                {
                    if (i + 1 < Size && program_text[i + 1] == '=')
                    {
                        Results.Add(new Lexeme(LexemeType.EQUAL, "==", Current_Row, Current_Col));
                        i += 2;
                        Current_Col += 2;
                    }
                    else
                    {
                        Results.Add(new Lexeme(LexemeType.ASSIGN, "=", Current_Row, Current_Col));
                        i += 1;
                        Current_Col += 1;
                    }
                }
                else if (Current_Character == '!' && i + 1 < Size && program_text[i + 1] == '=')
                {
                    Results.Add(new Lexeme(LexemeType.NOT_EQUAL,"!=",Current_Row,Current_Col));
                    i += 2;
                    Current_Col += 2;
                }
                else
                {
                    Results.Add(new Lexeme(
                    Current_Character switch
                    {
                        '{' => LexemeType.LEFT_BRACE,
                        '}' => LexemeType.RIGHT_BRACE,
                        '[' => LexemeType.LEFT_SQUARE_BRACKET,
                        ']' => LexemeType.RIGHT_SQUARE_BRACKET,
                        '(' => LexemeType.LEFT_ROUND_BRACKET,
                        ')' => LexemeType.RIGHT_ROUND_BRACKET,
                        '+' => LexemeType.PLUS,
                        '-' => LexemeType.MINUS,
                        '*' => LexemeType.MULTIPLY,
                        '/' => LexemeType.DIVIDE,
                        ';' => LexemeType.SEMICOLON,
                        ',' => LexemeType.COMMA,
                        _ => throw new Exception($"Неизвестный символ: '{Current_Character}', (row = '{Current_Row}' col = '{Current_Col}')")
                    },Current_Character.ToString(),Current_Row,Current_Col));
                    i += 1;
                    Current_Col += 1;
                }
               
            }
            Results.Add(new Lexeme(LexemeType.FINISH, "FINISH_LEXEME", Current_Row, Current_Col));
            return Results;
        }

        static bool IsChar(char ch)
        {
            return ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
        }
        static bool IsDigit(char ch)
        {
            return ('0' <= ch && ch <= '9');
        }
        static bool IsSpace(char ch)
        {
            return (ch == '\f') || (ch == '\n') || (ch == '\v') || (ch == '\r') || (ch == ' ');
        }
    }
}

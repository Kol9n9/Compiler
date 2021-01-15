using System;
using System.IO;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string file_name = "text.txt";
            try
            {
                string textFromFile = "";
                using(FileStream fstream = File.OpenRead(file_name))
                {
                    byte[] array = new byte[fstream.Length];
                    fstream.Read(array,0,array.Length);
                    textFromFile = System.Text.Encoding.Default.GetString(array);
                }
                Interpreter interpreter = new Interpreter(textFromFile);
                interpreter.RunInterpreter();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: '{0}'", e.Message);
            }
        }
    }
}

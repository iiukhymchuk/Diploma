using SetTheory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        const string Prompt = "> ";

        static void Main()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            string input;
            do
            {
                Console.ResetColor();
                Console.Write(Prompt);
                input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    WriteDescription();
                    continue;
                }

                DisplayEvaluationResult(input);

            } while (true);
        }

        static void DisplayEvaluationResult(string input)
        {
            try
            {
                var result = Pipeline.Process(input);
                WriteResult(input, result);
            }
            catch (Exception ex)
            {
                WriteRuntimeError(ex.Message);
            }
        }

        static void WriteResult(string input, Result<List<SimplificationDescription>> result)
        {
            if (result.HasValue)
                WriteResultLines(result.Value);
            else
                WriteSyntaxError(input, result.ErrorMessage, result.ErrorIndex, result.Token);
        }

        private static void WriteResultLines(IEnumerable<SimplificationDescription> lines)
        {
            foreach (var l in lines)
            {
                var message = $"{l.SimplifiedExpression,-30} | {l.AppliedRule} - {l.RuleDescription}";
                WriteInColor(message, ConsoleColor.Cyan);
            }
        }

        static void WriteSyntaxError(string input, string message, int inputErrorIndex, string token)
        {
            if (inputErrorIndex >= 0)
            {
                var promptSubstitute = new string(' ', Prompt.Length);
                var validPart = input.Substring(0, inputErrorIndex);
                var invalidPart = input.Substring(inputErrorIndex, token.Length);
                var rest = input.Substring(inputErrorIndex + token.Length);
                //var errorPointerLine = new string('-', Prompt.Length + inputErrorIndex) + '^';
                Console.Write($"{promptSubstitute}{validPart}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(invalidPart);
                Console.ResetColor();
                Console.WriteLine(rest);
            }
            WriteInColor(message, ConsoleColor.Yellow);
        }

        static void WriteRuntimeError(string message)
        {
            WriteInColor(message, ConsoleColor.Red);
        }

        static void WriteInColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        static void WriteDescription()
        {
            throw new NotImplementedException();
        }
    }
}
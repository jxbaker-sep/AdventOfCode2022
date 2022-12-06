using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;

namespace AdventOfCode2022.Utils
{
    public interface IAdventOfCode
    {
        void Run();
    }

    public static class AdventOfCodeExtensions
    {
        public static string File(this IAdventOfCode self) => System.IO.File.ReadAllText($"Days/{self.GetType().Name}/Input.txt");
        public static string Example(this IAdventOfCode self) => System.IO.File.ReadAllText($"Days/{self.GetType().Name}/Example.txt");
    }

    public abstract class AdventOfCode<TOut, TIn> : IAdventOfCode
    {
        public void Run()
        {
            var example = new List<TIn>();
            var file = new List<TIn>();

            var part1TestCases = GetType().GetMethod("Part1")!.GetCustomAttributes<TestCaseAttribute>().ToList();
            var part2TestCases = GetType().GetMethod("Part2")!.GetCustomAttributes<TestCaseAttribute>().ToList();

            if (part1TestCases.Union(part2TestCases).Any(it => it.Input == Input.Example))
            {
                example.Add(Parse(this.Example()));
            }

            if (part1TestCases.Union(part2TestCases).Any(it => it.Input == Input.File))
            {
                file.Add(Parse(this.File()));
            }

            foreach (var testCase in part1TestCases)
            {
                var actual = Part1(testCase.Input switch{
                    Input.Example => example[0],
                    Input.File => file[0],
                    Input.Raw => Parse(testCase.Raw),
                    _ => throw new ApplicationException()
                });
                if (!actual!.Equals(Coerce2nd(actual, testCase.Expected)))
                {
                    Console.WriteLine($"\nERROR! {this.GetType().Name}/Part 1/{testCase.Input} expected {testCase.Expected}, got {actual}");
                }
            }

            foreach (var testCase in part2TestCases)
            {
                var actual = Part2(testCase.Input switch{
                    Input.Example => example[0],
                    Input.File => file[0],
                    Input.Raw => Parse(testCase.Raw),
                    _ => throw new ApplicationException()
                });
                if (!actual!.Equals(Coerce2nd(actual, testCase.Expected)))
                {
                    Console.WriteLine($"\nERROR! {this.GetType().Name}/Part 2/{testCase.Input} expected {testCase.Expected}, got {actual}");
                }
            }
        }

        object? Coerce2nd(object n, object n2)
        {
            if (n2 == null) return n2;
            if (n2.GetType() == typeof(int) && n.GetType() == typeof(long))
            {
                return Convert.ToInt64(n2);
            }
            return n2;
        }

        public abstract TIn Parse(string input);

        public abstract TOut Part1(TIn input);
        public abstract TOut Part2(TIn input);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute: Attribute
    {
        public Input Input { get; }
        public object Expected { get; }
        public string Raw {get; set;} = "";

        public TestCaseAttribute(Input input, object expected)
        {
            Input = input;
            Expected = expected;
        }
    }

    public enum Input
    {
        Example,
        File,
        Raw
    }
}
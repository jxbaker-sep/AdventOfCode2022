﻿using System;
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

    public abstract class AdventOfCode<T> : IAdventOfCode
    {
        public void Run()
        {
            var example = new List<T>();
            var file = new List<T>();

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
                // TestCase = testCase;
                Part1(testCase.Input == Input.Example ? example[0] : file[0]).Should().Be(testCase.Expected);
            }
            foreach (var testCase in part2TestCases)
            {
                // TestCase = testCase;
                Part2(testCase.Input == Input.Example ? example[0] : file[0]).Should().Be(testCase.Expected);
            }
        }

        public abstract T Parse(string input);

        public abstract long Part1(T input);
        public abstract long Part2(T input);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute: Attribute
    {
        public Input Input { get; }
        public long Expected { get; }

        public TestCaseAttribute(Input input, long expected)
        {
            Input = input;
            Expected = expected;
        }
    }

    public enum Input
    {
        Example,
        File
    }
}
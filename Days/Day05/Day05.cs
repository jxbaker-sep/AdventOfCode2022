using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day05;

[UsedImplicitly]
public class Day05 : AdventOfCode<string, Day05Input>
{
    public override Day05Input Parse(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var (stackLines, iLines) = lines.SplitInTwo(line => line.Trim() == "");
        var stacks = Enumerable.Range(0, (lines.First() + " ").Count() / 4).Select(_ => new List<char>()).ToList();

        foreach(var line in stackLines)
        {
            if (line.StartsWith(" 1"))
            {
                continue;
            }
            foreach(var (group, index) in (line + " ").InGroupsOf(4).WithIndices())
            {
                if (group[1] != ' ') stacks[index].Insert(0, group[1]);
            }
        }

        return new(stacks, iLines.Parse<Day05Instruction>());
    }

    [TestCase(Input.Example, "CMZ")]
    [TestCase(Input.File, "SBPQRSCDF")]
    public override string Part1(Day05Input input)
    {
        var stacks = input.Stacks.Select(it => it.ToList()).ToList();
        foreach(var instruction in input.Instructions)
        {
            var l = stacks[instruction.From-1].PopRange(instruction.Count);
            l.Reverse();
            stacks[instruction.To-1].AddRange(l);
        }

        return stacks.Select(it => it.Last()).Join("");
    }

    [TestCase(Input.Example, "MCD")]
    [TestCase(Input.File, "RGLVRCQSB")]
    public override string Part2(Day05Input input)
    {
        var stacks = input.Stacks.Select(it => it.ToList()).ToList();
        foreach(var instruction in input.Instructions)
        {
            var l = stacks[instruction.From-1].PopRange(instruction.Count);
            stacks[instruction.To-1].AddRange(l);
        }

        return stacks.Select(it => it.Last()).Join("");
    }
}

public record Day05Instruction(
    [Before("move")] int Count,
    [Before("from")] int From,
    [Before("to")] int To
);
public record Day05Input(
    IReadOnlyList<IReadOnlyList<char>> Stacks,
    IReadOnlyList<Day05Instruction> Instructions);
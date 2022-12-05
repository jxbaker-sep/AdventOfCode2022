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
    private enum ParseState {Stacks, Instructions};
    public override Day05Input Parse(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var state = ParseState.Stacks;
        var stacks = Enumerable.Range(0, (lines.First() + " ").Count() / 4).Select(_ => new List<char>()).ToList();
        var instructions = new List<Day05Instruction>();


        foreach(var line in lines)
        {
            if (state == ParseState.Stacks)
            {
                if (line.StartsWith(" 1"))
                {
                    state = ParseState.Instructions;
                    continue;
                }
                foreach(var (group, index) in (line + " ").InGroupsOf(4).WithIndices())
                {
                    if (group[1] != ' ') stacks[index].Insert(0, group[1]);
                }
            }
            else
            {
                if (line.Trim() == "") continue;
                instructions.Add(TypeCompiler.Parse<Day05Instruction>(line));
            }
        }

        return new(stacks, instructions);
    }

    [TestCase(Input.Example, "CMZ")]
    [TestCase(Input.File, "SBPQRSCDF")]
    public override string Part1(Day05Input input)
    {
        var stacks = input.Stacks.Select(it => it.ToList()).ToList();
        foreach(var instruction in input.Instructions)
        {
            foreach (var _ in Enumerable.Range(0, instruction.Count))
            {
                var item = stacks[instruction.From-1].Pop();
                stacks[instruction.To-1].Add(item);
            }
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
            var items = stacks[instruction.From-1].Skip(stacks[instruction.From-1].Count - instruction.Count).ToList();
            stacks[instruction.From-1] = stacks[instruction.From-1].Take(stacks[instruction.From-1].Count - instruction.Count).ToList();
            stacks[instruction.To-1].AddRange(items);
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
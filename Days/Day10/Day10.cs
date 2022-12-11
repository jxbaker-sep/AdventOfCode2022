using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day10;

[UsedImplicitly]
public class Day10 : AdventOfCode<long, IReadOnlyList<Day10Input>>
{
    public override IReadOnlyList<Day10Input> Parse(string input) => TypeCompiler.ParseLines<Day10Input>(input);

    [TestCase(Input.Example, 13140)]
    [TestCase(Input.File, 13720)]
    public override long Part1(IReadOnlyList<Day10Input> input)
    {
        var states = ProduceStates(input);

        return new[] { 20, 60, 100, 140, 180, 220 }.Aggregate(0L,
            (accumulator, cycle) => accumulator + cycle * states[cycle - 1]);
    }

    // [TestCase(Input.Example, 0)]
    // [TestCase(Input.File, 0)]
    public override long Part2(IReadOnlyList<Day10Input> input)
    {
        var states = ProduceStates(input);
        var crt = Enumerable.Repeat(false, 40 * 6).ToList();

        foreach(var cycle in Enumerable.Range(0, 240))
        {
            var cursorPosition = cycle % 40;
            var middleOfSprite = states[cycle];
            if (cursorPosition >= middleOfSprite - 1 && cursorPosition <= middleOfSprite + 1)
            {
                crt[cycle] = true;
            }
        }
        Console.WriteLine();
        crt.InGroupsOf(40)
            .Select(x => x.Select(y => y ? "#" : ".").Join(""))
            .ToList()
            .ForEach(line => Console.WriteLine(line));

        return 0;
    }

    private static List<long> ProduceStates(IReadOnlyList<Day10Input> input)
    {
        var x = 1L;
        var states = new List<long> { 1 };
        foreach (var item in input)
        {
            states.Add(x);
            if (item.AddX is long l)
            {
                x += l;
                states.Add(x);
            }
        }

        return states;
    }
}

public record Day10Input(
    [Alternate, Format(Before = "addx")] long? AddX,
    [Alternate, Format(Before = "noop", Regex = "")] string? Noop
);
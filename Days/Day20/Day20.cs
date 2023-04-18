using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day20;

[UsedImplicitly]
public class Day20 : AdventOfCode<long, IReadOnlyList<int>>
{
    public override IReadOnlyList<int> Parse(string input) => input.Lines().Select(
        line => Convert.ToInt32(line)
    ).ToList();

    [TestCase(Input.Example, 3)]
    [TestCase(Input.File, 0)] // 7462 too high
    public override long Part1(IReadOnlyList<int> numbers)
    {
        var array = numbers.Select((it, index) => new {Value = it, OriginalIndex = index}).ToList();
        // Console.WriteLine();
        // Console.WriteLine(array.Csv());
        foreach(var originalIndex in numbers.WithIndices().Select(it => it.Index))
        {
            var index = array.FindIndex(item => item.OriginalIndex == originalIndex);
            var number = array[index].Value;
            var newIndex = LMath.MathMod(index + number, numbers.Count - 1);
            var temp = array.Take(index).Concat(array.Skip(index+1)).ToList();
            var newarray = temp.Take(newIndex).Append(new {Value = number, OriginalIndex = originalIndex}).Concat(temp.Skip(newIndex)).ToList();
            array = newarray;
            // Console.WriteLine(array.Csv());
        }
        var zeroIndex = array.FindIndex(it => it.Value == 0);
        return array[(zeroIndex + 1000) % numbers.Count].Value + 
            array[(zeroIndex + 2000) % numbers.Count].Value + 
            array[(zeroIndex + 3000) % numbers.Count].Value;
    }

    public override long Part2(IReadOnlyList<int> numbers)
    {
        return 0;
    }
}


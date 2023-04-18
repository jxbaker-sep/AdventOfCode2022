using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day20;

[UsedImplicitly]
public class Day20 : AdventOfCode<long, IReadOnlyList<long>>
{
    public override IReadOnlyList<long> Parse(string input) => input.Lines().Select(
        line => Convert.ToInt64(line)
    ).ToList();

    [TestCase(Input.Example, 3)]
    [TestCase(Input.File, 988)]
    public override long Part1(IReadOnlyList<long> numbers)
    {
        var array = Encrypt(numbers, 1);
        var zeroIndex = array.FindIndex(it => it == 0);
        return array[(zeroIndex + 1000) % numbers.Count] + 
            array[(zeroIndex + 2000) % numbers.Count] + 
            array[(zeroIndex + 3000) % numbers.Count];
    }

    [TestCase(Input.Example, 1623178306)]
    [TestCase(Input.File, 7768531372516)]
    public override long Part2(IReadOnlyList<long> numbers)
    {
        var array = Encrypt(numbers.Select(it => it * 811589153).ToList(), 10);
        var zeroIndex = array.FindIndex(it => it == 0);
        return array[(zeroIndex + 1000) % numbers.Count] + 
            array[(zeroIndex + 2000) % numbers.Count] + 
            array[(zeroIndex + 3000) % numbers.Count];
    }

    private IReadOnlyList<long> Encrypt(IReadOnlyList<long> numbers, int times)
    {
        var array = numbers.Select((it, index) => new {Value = it, OriginalIndex = index}).ToList();
        // Console.WriteLine();
        // Console.WriteLine(array.Csv());
        foreach(var _ in Enumerable.Range(0, times))
        foreach(var originalIndex in numbers.WithIndices().Select(it => it.Index))
        {
            var index = array.FindIndex(item => item.OriginalIndex == originalIndex);
            var number = array[index].Value;
            var newIndex = LMath.MathMod(index + number, numbers.Count - 1);
            var temp = array.Take(index).Concat(array.Skip(index+1)).ToList();
            var newarray = temp.Take((int)newIndex).Append(new {Value = number, OriginalIndex = originalIndex}).Concat(temp.Skip((int)newIndex)).ToList();
            array = newarray;
            // Console.WriteLine(array.Csv());
        }
        return array.Select(it => it.Value).ToList();
    }
}


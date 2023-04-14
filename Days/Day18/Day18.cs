using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day18;

[UsedImplicitly]
public class Day18 : AdventOfCode<long, List<int>>
{
    public override List<int> Parse(string input) => input.Split(",").Select(it => Convert.ToInt32(it)).ToList();


    [TestCase(Input.Raw, 0, Raw = "1,0,1,0,0")]
    public override long Part1(List<int> input)
    {
        var closed = new HashSet<string>();
        var open = new Queue<(List<int>, List<int>)>();
        open.Enqueue((input, new()));
        while (open.TryDequeue(out var current))
        {
            var (state, path) = current;
            var currentKey = state.Join(",");
            foreach(var (press, newState) in Open(state))
            {
                var newKey = newState.Join(",");
                if (newKey == "1,1,1,1,1")
                {
                    Console.WriteLine();
                    Console.WriteLine(path.Join(",") + "," + press);
                    return 0;
                }
                if (closed.Contains(newKey)) continue;
                closed.Add(newKey);
                open.Enqueue((newState, path.Append(press).ToList()));
            }
        }
        throw new ApplicationException();
    }

    IEnumerable<(int, List<int>)> Open(List<int> state)
    {
        yield return (0, new(){ T(state[0]), T(state[1]), T(state[2]), state[3], state[4] });
        yield return (1, new(){ T(state[0]), T(state[1]), T(state[2]), T(state[3]), state[4] });
        yield return (2, new(){ T(state[0]), T(state[1]), T(state[2]), T(state[3]), T(state[4]) });
        yield return (3, new(){ state[0], T(state[1]), T(state[2]), T(state[3]), T(state[4]) });
        yield return (4, new(){ state[0], state[1], T(state[2]), T(state[3]), T(state[4]) });
    }

    int T(int i) => i == 0 ? 1 : 0;


    public override long Part2(List<int> valves)
    {
        return 0;
    }
}

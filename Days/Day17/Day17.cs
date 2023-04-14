using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day17;

[UsedImplicitly]
public class Day17 : AdventOfCode<long, List<char>>
{
    public override List<char> Parse(string input) => input.Lines().Single().ToList();


    [TestCase(Input.Example, 3068)]
    [TestCase(Input.File, 0)]
    public override long Part1(List<char> input)
    {
        var rightWall = 8;
        var leftWall = 0;
        var floor = 0;

        var space = new HashSet<Position>();
        var windStep = 0;

        foreach (var rock in Enumerable.Range(0, 2022))
        {
            var pattern = Shapes.Patterns[rock % Shapes.Patterns.Count];

            var topRock = -space.Select(p => p.Y).Append(floor).Min();


            pattern = Shapes.Move(pattern, Vector.East * 3 + Vector.North * (topRock + 3 + Shapes.Height(pattern)) );

            foreach(var step in Enumerable.Range(0, int.MaxValue))
            {
                var wind = input[(windStep++) % (input.Count)] switch {
                    '<' => Vector.West,
                    '>' => Vector.East,
                    _ => throw new ApplicationException()
                };

                var newPattern = Shapes.Move(pattern, wind);
                if (newPattern.All(p => !space.Contains(p) &&
                    Shapes.Left(newPattern) > leftWall && 
                    Shapes.Right(newPattern) < rightWall))
                {
                    pattern = newPattern;
                }

                newPattern = Shapes.Move(pattern, Vector.South);
                if (newPattern.All(p => !space.Contains(p) && p.Y < floor))
                {
                    pattern = newPattern;
                    continue;
                }
                foreach(var p in pattern)
                {
                    space.Add(p);
                }
                // PrintGrid(space);
                break;
            }
        }

        return space.Select(p => -p.Y).Max();
    }

    private void PrintGrid(HashSet<Position> grid)
    {
        Console.WriteLine("........");
        for(var y = grid.Select(p => p.Y).Min(); y <= grid.Select(p => p.Y).Max(); y++)
        {
            for(var x = grid.Select(p => p.X).Min(); x <= grid.Select(p => p.X).Max(); x++)
            {
                Console.Write(grid.Contains(new(y,x)) ? '#' : ' ');
            }
            Console.WriteLine();
        }
    }


    [TestCase(Input.Example, 1514285714288L)]
    // [TestCase(Input.File, 2351)]
    public override long Part2(List<char> valves)
    {
        return 0;
    }

    public static class Shapes
    {
        public static IReadOnlySet<Position> HLine = new HashSet<Position>(){new(0,0), new(0, 1), new(0, 2), new(0,3)};
        public static IReadOnlySet<Position> Plus = new HashSet<Position>(){new(0,1), new(1, 0), new(1, 1), new(1, 2), new(2, 1)};

        public static IReadOnlySet<Position> El = new HashSet<Position>(){new(0,2), new(1, 2), new(2, 0), new(2, 1), new(2,2)};

        public static IReadOnlySet<Position> VLine = new HashSet<Position>(){new(0,0), new(1, 0), new(2, 0), new(3,0)};

        public static IReadOnlySet<Position> Square = new HashSet<Position>(){new(0,0), new(0, 1), new(1, 0), new(1, 1)};

        public static IReadOnlyList<IReadOnlySet<Position>> Patterns => new List<IReadOnlySet<Position>>()
        {
            HLine, Plus, El, VLine, Square
        };

        public static IReadOnlySet<Position> Move(IReadOnlySet<Position> pattern, Vector v)
        {
            return pattern.Select(p => p + v).ToHashSet();
        }

        public static long Height(IReadOnlySet<Position> pattern)
        {
            return 1 + pattern.Select(p => p.Y).Max() - pattern.Select(p => p.Y).Min();
        }

        public static long Left(IReadOnlySet<Position> pattern)
        {
            return pattern.Select(p => p.X).Min();
        }

        public static long Right(IReadOnlySet<Position> pattern)
        {
            return pattern.Select(p => p.X).Max();
        }
    }
}

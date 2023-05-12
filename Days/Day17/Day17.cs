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
    private const int rightWall = 8;
    private const int leftWall = 0;

    [TestCase(Input.Example, 3068)]
    [TestCase(Input.File, 3144)]
    public override long Part1(List<char> input)
    {
        return Execute(input, 2022);
    }

    [TestCase(Input.Example, 1514285714288L)]
    [TestCase(Input.File, 1565242165201L)]
    public override long Part2(List<char> input)
    {
        return Execute(input, 1000000000000);
    }

    private long Execute(List<char> input, long iterations)
    {
        var space = new NormalizedSpace(0, new HashSet<Position> 
        {  
            new Position(0, 0),
            new Position(0, 1),
            new Position(0, 2),
            new Position(0, 3),
            new Position(0, 4),
            new Position(0, 5),
            new Position(0, 6),
            new Position(0, 7),
            new Position(0, 8),
        }, 0);
        var visited = new Dictionary<(string, int, int), (NormalizedSpace Space, long Iteration)>();
        var finishing = false;
        for (var iteration = 0L; iteration < iterations ; )
        {
            var key = (SpaceCode(space.Space), space.Windstep, (int)(iteration % Shapes.Patterns.Count));
            if (!finishing && !visited.TryAdd(key, (space, iteration)))
            {
                var previous = visited[key];
                var n = (iterations - iteration) / (iteration - previous.Iteration);
                // Console.WriteLine($"\n{n}, {space.Floor - previous.Space.Floor}, {space.Floor}, {space.Floor + (space.Floor - previous.Space.Floor) * n}, {space.Space.Select(p => -p.Y).Max()}, {iteration}, {(iteration - previous.Iteration)}, {iteration + n * (iteration - previous.Iteration)}");
                iteration += n * (iteration - previous.Iteration);
                
                finishing = true;
                space = new NormalizedSpace(space.Floor + (space.Floor - previous.Space.Floor) * n, space.Space, space.Windstep);
                continue;
            }
            space = DropRock(input, space, iteration);
            iteration += 1;
        }

        return space.Space.Select(p => -p.Y).Max() + space.Floor;
    }

    private string SpaceCode(IReadOnlySet<Position> space) => space.OrderBy(p => p.Y).ThenBy(p => p.X).Select(p => p.ToString()).Join();

    private NormalizedSpace DropRock(List<char> input, NormalizedSpace normalizedSpace, long rock)
    {
        var pattern = Shapes.Patterns[(int)(rock % Shapes.Patterns.Count)];
        var windstep = normalizedSpace.Windstep;
        var space = normalizedSpace.Space.ToHashSet();

        var topRock = -space.Select(p => p.Y).Min();

        pattern = Shapes.Move(pattern, Vector.East * 3 + Vector.North * (topRock + 3 + Shapes.Height(pattern)));

        foreach (var step in Enumerable.Range(0, int.MaxValue))
        {
            var wind = input[(windstep++) % (input.Count)] switch
            {
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
            if (newPattern.All(p => !space.Contains(p)))
            {
                pattern = newPattern;
                continue;
            }
            foreach (var p in pattern)
            {
                space.Add(p);
            }
            // PrintGrid(space);
            break;
        }

        return CreateNormalizedSpace(normalizedSpace.Floor, space, windstep % (input.Count));
    }

    // private void PrintGrid(HashSet<Position> grid)
    // {
    //     Console.WriteLine("........");
    //     for(var y = grid.Select(p => p.Y).Min(); y <= grid.Select(p => p.Y).Max(); y++)
    //     {
    //         for(var x = grid.Select(p => p.X).Min(); x <= grid.Select(p => p.X).Max(); x++)
    //         {
    //             Console.Write(grid.Contains(new(y,x)) ? '#' : ' ');
    //         }
    //         Console.WriteLine();
    //     }
    // }




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

    private NormalizedSpace CreateNormalizedSpace(long floor, HashSet<Position> space, int windstep)
    {
        var minY = space.Select(p => p.Y).Min() - 1;
        var start = new Position(minY, leftWall + 1);
        var open = new Queue<Position>();
        open.Enqueue(start);
        var closed = new HashSet<Position>();
        var reachableBlocks = new HashSet<Position>();

        while (open.TryDequeue(out var current))
        {
            foreach(var neighbor in current.OrthoganalNeighbors())
            {
                if (neighbor.X <= leftWall || neighbor.X >= rightWall) continue;
                if (space.Contains(neighbor))
                {
                    reachableBlocks.Add(neighbor);
                    continue;
                }
                if (neighbor.Y < minY || !closed.Add(neighbor)) continue;
                open.Enqueue(neighbor);
            }
        }

        var maxY = reachableBlocks.Select(p => p.Y).Max();
        reachableBlocks = reachableBlocks.Select(p => new Position(p.Y - maxY, p.X)).ToHashSet();
        var dy = space.Select(p => p.Y).Max() - maxY;

        return new NormalizedSpace(floor + dy, reachableBlocks, windstep);
    }
}

public record NormalizedSpace(long Floor, IReadOnlySet<Position> Space, int Windstep);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day19;

[UsedImplicitly]
public class Day19 : AdventOfCode<long, IReadOnlyList<Blueprint>>
{
    public override IReadOnlyList<Blueprint> Parse(string input)
    {
        var result = new List<Blueprint>();
        foreach (var line in input.Lines())
        {
            var m = Regex.Match(line, @"Blueprint (?<id>\d+): Each ore robot costs (?<oreOre>\d+) ore. Each clay robot costs (?<clayOre>\d+) ore. Each obsidian robot costs (?<obOre>\d+) ore and (?<obClay>\d+) clay. Each geode robot costs (?<geodeOre>\d+) ore and (?<geodeOb>\d+) obsidian.");
            if (!m.Success) throw new ApplicationException();
            result.Add(
                new Blueprint(m.LongGroup("id"),
                    new(m.LongGroup("oreOre"), 0, 0),
                    new(m.LongGroup("clayOre"), 0, 0),
                    new(m.LongGroup("obOre"), m.LongGroup("obClay"), 0),
                    new(m.LongGroup("geodeOre"), 0, m.LongGroup("geodeOb"))));
        }
        return result;
    }


    long MaxOre;
    long MaxClay;
    long MaxObsidian;
    long MaxGeodes;
    [TestCase(Input.Example, 33)]
    [TestCase(Input.File, 1365)]
    public override long Part1(IReadOnlyList<Blueprint> blueprints)
    {
        var current = 0L;
        foreach (var blueprint in blueprints)
        {
            current += Results(blueprint, 24).Max() * blueprint.Id;
        }
        return current;
    }

    [TestCase(Input.Example, 56 * 62)]
    [TestCase(Input.File, 4864)]
    public override long Part2(IReadOnlyList<Blueprint> blueprints)
    {
        var current = 1L;
        foreach (var blueprint in blueprints.Take(3))
        {
            var temp = Results(blueprint, 32).Max();
            current *= temp;
        }
        return current;
    }


    private IEnumerable<long> Results(Blueprint blueprint, int maxMinutes)
    {
        MaxOre = new[]{blueprint.clayRobot.ore, blueprint.oreRobot.ore, blueprint.geodeRobot.ore, blueprint.obsidianRobot.ore}.Max();
        MaxClay = new[]{blueprint.clayRobot.clay, blueprint.oreRobot.clay, blueprint.geodeRobot.clay, blueprint.obsidianRobot.clay}.Max();
        MaxObsidian = new[]{blueprint.clayRobot.obsidian, blueprint.oreRobot.obsidian, blueprint.geodeRobot.obsidian, blueprint.obsidianRobot.obsidian}.Max();
        MaxGeodes = 0;

        var open = new Stack<State>();
        open.Push(DefaultState);
        while (open.TryPop(out var current))
        {
            foreach (var substate in Open(blueprint, current, maxMinutes))
            {
                if (substate.Minute == maxMinutes) {
                    MaxGeodes = Math.Max(MaxGeodes, substate.geodes);
                    yield return substate.geodes;
                }
                else {
                    var theoreticalMax = substate.geodes + Enumerable.Range((int)substate.geodeRobots, (int)(maxMinutes - substate.Minute)).Sum();
                    if (theoreticalMax <= MaxGeodes) continue;
                    open.Push(substate);
                }
            }
        }
    }

    private IEnumerable<State> Open(Blueprint blueprint, State currentState, int maxMinutes)
    {
        if (currentState.obsidianRobots >= blueprint.geodeRobot.obsidian && currentState.oreRobots >= blueprint.geodeRobot.ore)
        {
            var minutes = maxMinutes - currentState.Minute;
            yield return currentState with { 
                ore = currentState.ore + currentState.oreRobots * minutes, 
                clay = currentState.clay + currentState.clayRobots * minutes, 
                obsidian = currentState.obsidian + currentState.obsidianRobots * minutes, 
                geodes = currentState.geodes + Enumerable.Range((int)currentState.geodeRobots, (int)minutes).Sum(),
                Minute = maxMinutes
            };

            yield break;
        }

        var none = true;
        if (currentState.obsidianRobots > 0)
        {
            var geodes = GenerateRobot(blueprint.geodeRobot, currentState, Resource.Geode, maxMinutes).ToList();
            foreach(var temp in geodes) 
            {
                none = false;
                yield return temp;
            }
        }
        
        if (currentState.clayRobots > 0 && currentState.obsidianRobots < MaxObsidian)
        {
            foreach(var temp in GenerateRobot(blueprint.obsidianRobot, currentState, Resource.Obsidian, maxMinutes)) 
            {
                none = false;
                yield return temp;
            }
        }
        if (currentState.clayRobots < MaxClay) foreach(var temp in GenerateRobot(blueprint.clayRobot, currentState, Resource.Clay, maxMinutes)) 
        {
            none = false;
            yield return temp;
        }
        if (currentState.oreRobots < MaxOre) foreach(var temp in GenerateRobot(blueprint.oreRobot, currentState, Resource.Ore, maxMinutes)) 
        {
            none = false;
            yield return temp;
        }

        if (none && currentState.geodeRobots > 0)
        {
            var minutes = maxMinutes - currentState.Minute;
            yield return currentState with { 
                ore = currentState.ore + currentState.oreRobots * minutes, 
                clay = currentState.clay + currentState.clayRobots * minutes, 
                obsidian = currentState.obsidian + currentState.obsidianRobots * minutes, 
                geodes = currentState.geodes + currentState.geodeRobots * minutes,
                Minute = maxMinutes
            };
        }
    }

    private IEnumerable<State> GenerateRobot(RobotCost cost, State currentState, Resource r, int maxMinutes)
    {
        var minutesToObsidian = currentState.obsidian >= cost.obsidian ? 1 : 1 + LMath.RoundUp(cost.obsidian - currentState.obsidian, currentState.obsidianRobots);
        var minutesToClay = currentState.clay >= cost.clay ? 1 : 1 + LMath.RoundUp(cost.clay - currentState.clay, currentState.clayRobots);
        var mintuesToOre = currentState.ore >= cost.ore ? 1 : 1 + LMath.RoundUp(cost.ore - currentState.ore, currentState.oreRobots);
        var minutes = new[]{minutesToClay, mintuesToOre, minutesToObsidian}.Max();
        if (minutes + currentState.Minute <= maxMinutes)
        {
            yield return currentState with { 
                ore = currentState.ore + currentState.oreRobots * minutes - cost.ore, 
                clay = currentState.clay + currentState.clayRobots * minutes - cost.clay, 
                obsidian = currentState.obsidian + currentState.obsidianRobots * minutes - cost.obsidian, 
                geodes = currentState.geodes + currentState.geodeRobots * minutes,
                obsidianRobots = currentState.obsidianRobots + (r == Resource.Obsidian ? 1 : 0),
                oreRobots = currentState.oreRobots + (r == Resource.Ore ? 1 : 0),
                clayRobots = currentState.clayRobots + (r == Resource.Clay ? 1 : 0),
                geodeRobots = currentState.geodeRobots + (r == Resource.Geode ? 1 : 0),
                Minute = currentState.Minute + minutes
            };
        }
    }

    private State DefaultState => new State(0, 1, 0, 0, 0, 0, 0, 0, 0);
}

enum Resource { Ore, Clay, Obsidian, Geode };

public record Blueprint(long Id, RobotCost oreRobot, RobotCost clayRobot, RobotCost obsidianRobot, RobotCost geodeRobot);

public record RobotCost(long ore, long clay, long obsidian);

public record State(long Minute, long oreRobots, long clayRobots, long obsidianRobots, long geodeRobots, long ore, long clay, long obsidian, long geodes);

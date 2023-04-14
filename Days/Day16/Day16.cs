using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2022.Days.Day16;

[UsedImplicitly]
public class Day16 : AdventOfCode<long, List<Valve>>
{
    public override List<Valve> Parse(string input) => input.Lines().Parse<Valve>();


    [TestCase(Input.Example, 1651)]
    [TestCase(Input.File, 1775)]
    public override long Part1(List<Valve> valves)
    {
        IReadOnlyList<Valve> meaningful = valves.Where(valve => valve.FlowRate > 0).ToList();
        var distances = CreateDistances(valves);

        var open = new Queue<State>();
        open.Enqueue(new(valves.Single(it => it.Name == "AA"), 1, 0, new List<Valve>()));
        var highest = 0L;
        while (open.TryDequeue(out var current))
        {
            var remaining = meaningful.ExceptBy(current.openValves.Select(v => v.Name), v => v.Name).ToList();
            if (current.Minute > 30) throw new ApplicationException();
            var currentFlowRate = current.openValves.Sum(v => v.FlowRate);
            var now = current.Minute;
            if (remaining.Count == 0 || now == 30)
            {
                var result = current.TotalReleasedPressure + (30 - now) * currentFlowRate;
                highest = Math.Max(highest, result);
                continue;
            }
            
            foreach(var destination in remaining)
            {
                var elapsed = distances[current.Valve.Name][destination.Name];
                if (now + elapsed + 1 > 30)
                {
                    var result = current.TotalReleasedPressure + (30 - now) * currentFlowRate;
                    highest = Math.Max(highest, result);
                    continue;
                }
                var trp = current.TotalReleasedPressure + (elapsed + 1) * currentFlowRate + destination.FlowRate;
                open.Enqueue(new(destination, now + elapsed + 1, trp, current.openValves.Append(destination).ToList()));
            }
        }

        return highest;
    }


    [TestCase(Input.Example, 1707)]
    // [TestCase(Input.File, 2351)]
    public override long Part2(List<Valve> valves)
    {
        IReadOnlyList<Valve> meaningful = valves.Where(valve => valve.FlowRate > 0).ToList();
        var distances = CreateDistances(valves);

        var open = new Stack<State2>();
        var aa = valves.Single(it => it.Name == "AA");
        open.Push(new(aa, aa, 0, 0, 1, 0, new List<Valve>()));
        var highest = 0L;
        while (open.TryPop(out var current))
        {
            if (current.Minute > 26) throw new ApplicationException();
            var currentFlowRate = current.OpenValves.Sum(v => v.FlowRate);
            var now = current.Minute;
            if (meaningful.Count == current.OpenValves.Count || now == 26)
            {
                var result = current.TotalReleasedPressure + (26 - now) * currentFlowRate;
                if (result > highest)
                {
                    // Console.WriteLine(highest);
                    highest = result;
                }
                continue;
            }

            var maxPossible = current.TotalReleasedPressure + meaningful.Sum(v => v.FlowRate) * (26 - current.Minute);
            if (maxPossible < highest) continue;


            var remaining = meaningful
                .ExceptBy(current.OpenValves.Select(v => v.Name), v => v.Name)
                .Where(it => it.Name != current.MyValve.Name && it.Name != current.OtherValve.Name)
                .ToList();


            var futureRendezvous = new[]{current.MyRendezvous, current.OtherRendezvous}.Where(it => it > now).ToList();
            var presentRendezvous = new[]{current.MyRendezvous, current.OtherRendezvous}.Where(it => it == now).ToList();

            if (futureRendezvous.Count == 2 || 
                (remaining.Count == 0 && presentRendezvous.Count == 0)
            )
            {
                var jump = futureRendezvous.Append(26).Min() - now;
                open.Push(current with { Minute = now + jump, 
                    TotalReleasedPressure = current.TotalReleasedPressure + currentFlowRate * jump });
                continue;
            }

            if ((presentRendezvous.Count + futureRendezvous.Count == 2) || 
                (remaining.Count == 0 && (presentRendezvous.Count > 0))
            )
            {
                // At least one is equal to now
                var openValves = current.OpenValves.ToList();

                if (current.MyRendezvous == now)
                {
                    openValves.Add(current.MyValve);
                }

                if (current.OtherRendezvous == now)
                {
                    openValves.Add(current.OtherValve);
                }

                open.Push(current with { Minute = now + 1, 
                    TotalReleasedPressure = current.TotalReleasedPressure + openValves.Sum(v => v.FlowRate),
                    OpenValves = openValves });
                continue;
            }

            // At least one person can move
            if (remaining.Count == 0)
            {
                throw new ApplicationException();
                // // But they cant because there are no available destinations
                // var nextRendezvous = Math.Min(current.MyRendezvous, current.OtherRendezvous);
                // var jump = 26 - now;
                // if (nextRendezvous > now) jump = nextRendezvous - now;
                // open.Push(current with {
                //     Minute = now + jump,
                //     TotalReleasedPressure = current.TotalReleasedPressure + currentFlowRate * jump
                // });
                // continue;
            }
            
            if (current.MyRendezvous < now)
            {
                foreach(var destination in remaining)
                {
                    var driveTime = distances[current.MyValve.Name][destination.Name];
                    open.Push(current with { MyRendezvous = now + driveTime, MyValve = destination });
                }
            }
            
            if (current.OtherRendezvous < now)
            {
                foreach(var destination in remaining)
                {
                    var driveTime = distances[current.OtherValve.Name][destination.Name];
                    open.Push(current with { OtherRendezvous = now + driveTime, OtherValve = destination });
                }
            }
        }

        return highest;
    }

    private IReadOnlyDictionary<string, IReadOnlyDictionary<string, long>> CreateDistances(List<Valve> valves)
    {
        var result = new Dictionary<string, IReadOnlyDictionary<string, long>>();
        var valveMap = valves.ToDictionary(it => it.Name, it => it);
        var meaningful = valves.Where(valve => valve.FlowRate > 0).ToList();
        foreach(var valve in meaningful)
        {
            result.Add(valve.Name, CreateDistance(valve, valveMap));
        }
        var aa = valves.Single(it => it.Name == "AA");
        result.Add("AA", CreateDistance(aa, valveMap));
        return result;
    }

    private IReadOnlyDictionary<string, long> CreateDistance(Valve origin, IReadOnlyDictionary<string, Valve> destinations)
    {
        var open = new Queue<Valve>();
        open.Enqueue(origin);
        var result = new Dictionary<string, long>{ { origin.Name, 0 } };
        var closed = new HashSet<string>{ origin.Name };

        while (open.TryDequeue(out var current))
        {
            var distance = result[current.Name] + 1;
            foreach(var tunnel in current.Tunnels.Except(closed).ToList())
            {
                var next = destinations[tunnel];
                result.Add(next.Name, distance);
                closed.Add(tunnel);
                open.Enqueue(next);
            }
        }

        return result;
    }
}

public record State(Valve Valve, long Minute, long TotalReleasedPressure, IReadOnlyList<Valve> openValves);

public record State2(Valve MyValve, Valve OtherValve,
    long MyRendezvous, long OtherRendezvous,
    long Minute, long TotalReleasedPressure, IReadOnlyList<Valve> OpenValves);

public record Valve(
    [Before("Valve")] string Name,
    [Before("has flow rate=")] long FlowRate,
    [Format(BeforeRx="; tunnel(s?) lead(s?) to valve(s?)", Separator = ",")] IReadOnlyList<string> Tunnels
);
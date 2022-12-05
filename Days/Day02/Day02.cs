using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2022.Days.Day02;

[UsedImplicitly]
public class Day02 : AdventOfCode<long, List<Day02Input>>
{
    public override List<Day02Input> Parse(string input) => input
        .Lines()
        .Select(line => new Day02Input(CharToRPS(line[0]), CharToRPS(line[2]), CharToWLD(line[2]))).ToList();

    [TestCase(Input.Example, 15)]
    [TestCase(Input.File, 12679)]
    public override long Part1(List<Day02Input> input)
    {
        return input.Select(game => ScorePlayer(game.Player) + ScoreGame(game)).Sum();
    }

    [TestCase(Input.Example, 12)]
    [TestCase(Input.File, 14470)]
    public override long Part2(List<Day02Input> input)
    {
        return input
            .Select(game => game with { Player = Transform(game) } )
            .Select(game => ScorePlayer(game.Player) + ScoreGame(game))
            .Sum();
    }

    private static RPS Transform(Day02Input game)
    {
        var beatenBy = new Dictionary<RPS, RPS>
        {
            { RPS.Paper, RPS.Scissors },
            { RPS.Rock, RPS.Paper },
            { RPS.Scissors, RPS.Rock },
        };
        var beats = beatenBy.ToDictionary(k => k.Value, v => v.Key);
        return game.winLoseDraw switch
        {
            WLD.Draw => game.Opponent,
            WLD.Lose => beats[game.Opponent],
            _ => beatenBy[game.Opponent]
        };
    }

    private static long ScorePlayer(RPS x)
    {
        return x switch
        {
            RPS.Rock => 1,
            RPS.Paper => 2,
            RPS.Scissors => 3,
            _ => throw new Exception()
        };
    }

    private static long ScoreGame(Day02Input input)
    {
        var beatenBy = new Dictionary<RPS, RPS>
        {
            { RPS.Paper, RPS.Scissors },
            { RPS.Rock, RPS.Paper },
            { RPS.Scissors, RPS.Rock },
        };

        if (beatenBy[input.Opponent] == input.Player) return 6;
        if (input.Opponent == input.Player) return 3;
        return 0;
    }

    private static RPS CharToRPS(char x)
    {
        return x switch
        {
            'X' => RPS.Rock,
            'A' => RPS.Rock,
            'Y' => RPS.Paper,
            'B' => RPS.Paper,
            'Z' => RPS.Scissors,
            'C' => RPS.Scissors,
            _ => throw new Exception()
        };
    }

    private static WLD CharToWLD(char x)
    {
        return x switch
        {
            'X' => WLD.Lose,
            'Y' => WLD.Draw,
            'Z' => WLD.Win,
            _ => throw new Exception()
        };
    }
}

public enum RPS { Rock, Paper, Scissors }

public enum WLD { Win, Lose, Draw }

public record Day02Input(RPS Opponent, RPS Player, WLD winLoseDraw);
using System;
using System.Linq;
using System.Reflection;
using AdventOfCode2022.Days.Day01;
using AdventOfCode2022.Utils;
using TypeParser;

namespace AdventOfCode2022
{
    class Program
    {
        static void Main(string[] args)
        {
            var regression = args.Contains("--regression") || args.Contains("--r");

            var days = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => !type.IsAbstract)
                .Where(type => type.IsClass)
                .Where(type => type.IsAssignableTo(typeof(IAdventOfCode)))
                .Select(type => (type, TypeCompiler.ParseOrDefault<DayClass>(type.Name)))
                .Where(it => it.Item2 != null)
                .OrderBy(it => it.Item2!.DayNumber)
                .ToList();

            if (!regression)
            {
                days = EnumerableExtensions.ListFromItem(days.Last());
            }

            foreach (var day in days)
            {
                Console.Write(day.type.Name);
                var instance = (IAdventOfCode)day.type.GetConstructor(new Type[] { })!.Invoke(new object?[] { });
                var start = DateTime.Now;
                instance.Run();
                var stop = DateTime.Now;
                Console.WriteLine($"  {(stop - start).TotalSeconds:N3}s");
            }
        }
    }

    public class DayClass
    {
        [Format(Before = "Day")]
        public int DayNumber { get; set; }
    }
}
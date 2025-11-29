using System.Diagnostics;

namespace FileIoBench
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rng = new Random();
            var randomTexts = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                randomTexts.Add(Guid.NewGuid().ToString());
            }
            string benchDir = "c:\\tmp\\io_bench";

            List<double> measurements = new();
            for (int i = 0; i < 20; i++)
            {
                var took = NewMethod(randomTexts, benchDir);
                measurements.Add(took.TotalSeconds);
            }

            Console.WriteLine($"mean: {measurements.Average():0.00} s");
            Console.WriteLine($"median: {measurements.Order().Skip(measurements.Count/2).First():0.00} s");
        }

        private static TimeSpan NewMethod(List<string> randomTexts, string benchDir)
        {
            if (Directory.Exists(benchDir))
            {
                Directory.Delete(benchDir, true);
            }

            Directory.CreateDirectory(benchDir);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < randomTexts.Count; i++)
            {
                File.WriteAllText($"{benchDir}\\file_{i}.txt", randomTexts[i]);
            }

            int sum = 0;
            foreach (var file in Directory.GetFiles(benchDir))
            {
                sum += File.ReadAllText(file).Length; // prevent compiler to optimize it away
            }

            sw.Stop();
            Console.WriteLine($"{sw.Elapsed.TotalSeconds} s");

            return sw.Elapsed;
        }
    }
}

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

            // warmup
            for (int i = 0; i < 3; i++)
            {
                var took = RunBench(randomTexts, benchDir);
                Print("warmup", i, took);
            }

            List<double> measurements = new();
            for (int i = 0; i < 20; i++)
            {
                var took = RunBench(randomTexts, benchDir);
                Print("run", i, took);
                measurements.Add(took.Read.TotalSeconds + took.Write.TotalSeconds);
            }

            Console.WriteLine($"mean: {measurements.Average():0.00} s");
            Console.WriteLine($"median: {measurements.Order().Skip(measurements.Count/2).First():0.00} s");
        }

        private static void Print(string label, int i, (TimeSpan Write, TimeSpan Read) took)
        {
            Console.WriteLine($"{label} {i:00}: write {took.Write.TotalSeconds:f2} s, read: {took.Read.TotalSeconds:f3} s, total: {took.Write.TotalSeconds + took.Read.TotalSeconds:f2} s");
        }

        private static (TimeSpan Write, TimeSpan Read) RunBench(List<string> randomTexts, string benchDir)
        {
            if (Directory.Exists(benchDir))
            {
                Directory.Delete(benchDir, true);
            }

            Directory.CreateDirectory(benchDir);

            var swWrite = Stopwatch.StartNew();
            for (int i = 0; i < randomTexts.Count; i++)
            {
                File.WriteAllText($"{benchDir}\\file_{i}.txt", randomTexts[i]);
            }
            swWrite.Stop();

            var swRead = Stopwatch.StartNew();
            int sum = 0;
            foreach (var file in Directory.GetFiles(benchDir))
            {
                sum += File.ReadAllText(file).Length; // prevent compiler to optimize it away
            }
            swRead.Stop();

            return (swWrite.Elapsed, swRead.Elapsed);
        }
    }
}

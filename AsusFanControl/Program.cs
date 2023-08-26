using System;
using AsusSystemAnalysis;

namespace AsusFanControl
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: AsusFanControl <fanSpeedPercent>");
                return 1;
            }

            bool tryParse = int.TryParse(args[0], out int num);

            if (!tryParse)
            {
                Console.WriteLine("Please enter a numeric value for fan speed.");
                return 1;
            }

            if ((num != 0 && num < 40) || num > 99)
            {
                Console.WriteLine("Please enter a value for fan speed between 40 and 99 or 0 for turning off test mode.");
                Console.WriteLine("If you want to skip these limits you need to change it in source code.");
                return 1;
            }

            var asusControl = new AsusControl();

            var fanSpeeds = asusControl.GetFanSpeeds();
            Console.WriteLine($"Current fan speeds: {string.Join(" ", fanSpeeds)}");

            asusControl.SetFansSpeed(num);

            return 0;
        }
    }
}

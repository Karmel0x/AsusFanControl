using System;

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
                Console.WriteLine("Usage: AsusFanControl <args>");
                Console.WriteLine("\t--get-fan-speeds");
                Console.WriteLine("\t--set-fan-speeds=0-100 (percent value, 0 for turning off test mode)");
                Console.WriteLine("\t--get-fan-count");
                Console.WriteLine("\t--get-fan-speed=fanId (comma separated)");
                Console.WriteLine("\t--set-fan-speed=fanId:0-100 (comma separated, percent value, 0 for turning off test mode)");
                Console.WriteLine("\t--get-cpu-temp");
                return 1;
            }

            var asusControl = new AsusControl();

            foreach (var arg in args)
            {
                if (arg.StartsWith("--get-fan-speeds"))
                {
                    var fanSpeeds = asusControl.GetFanSpeeds();
                    Console.WriteLine($"Current fan speeds: {string.Join(" ", fanSpeeds)} RPM");
                }

                if (arg.StartsWith("--set-fan-speeds"))
                {
                    var newSpeedStr = arg.Split('=')[1];
                    var newSpeed = int.Parse(newSpeedStr);
                    asusControl.SetFanSpeeds(newSpeed);

                    if(newSpeed == 0)
                        Console.WriteLine("Test mode turned off");
                    else
                        Console.WriteLine($"New fan speeds: {newSpeed}%");
                }

                if (arg.StartsWith("--get-fan-speed="))
                {
                    var fanIds = arg.Split('=')[1].Split(',');
                    foreach (var fanIdStr in fanIds)
                    {
                        var fanId = int.Parse(fanIdStr);
                        var fanSpeed = asusControl.GetFanSpeed((byte)fanId);
                        Console.WriteLine($"Current fan speed for fan {fanId}: {fanSpeed} RPM");
                    }
                }

                if (arg.StartsWith("--get-fan-count"))
                {
                    var fanCount = asusControl.HealthyTable_FanCounts();
                    Console.WriteLine($"Fan count: {fanCount}");
                }

                if (arg.StartsWith("--set-fan-speed="))
                {
                    var fanSettings = arg.Split('=')[1].Split(',');
                    foreach (var fanSetting in fanSettings)
                    {
                        var fanId = int.Parse(fanSetting.Split(':')[0]);
                        var fanSpeed = int.Parse(fanSetting.Split(':')[1]);
                        asusControl.SetFanSpeed(fanSpeed, (byte)fanId);

                        if (fanSpeed == 0)
                            Console.WriteLine($"Test mode turned off for fan {fanId}");
                        else
                            Console.WriteLine($"New fan speed for fan {fanId}: {fanSpeed}%");
                    }
                }

                if (arg.StartsWith("--get-cpu-temp"))
                {
                    var cpuTemp = asusControl.Thermal_Read_Cpu_Temperature();
                    Console.WriteLine($"Current CPU temp: {cpuTemp}");
                }
            }

            return 0;
        }
    }
}

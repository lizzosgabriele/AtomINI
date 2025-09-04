using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AtomINI {
    internal class Program {
        
        static LoggingLevelSwitch loggingLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);

        
        public static void Main(string[] args) {
            
            Logger log = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(loggingLevelSwitch)
                //.MinimumLevel.Verbose()
                .Enrich.WithProperty("SourceContext", "ILFashion")
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} [{SourceContext}] [{Level:u}] - {Message}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "ILFashion-.log", 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} [{SourceContext}] [{Level:u}] - {Message}{NewLine}{Exception}"
                )
                .CreateLogger();
            
            Log.Logger = log;
            
            
            string iniFilePath = "C:\\ATOM\\Logs\\Test\\test.ini";
            
            AtomIniSettings.enableExtLogging = false;
            AtomIniSettings.useMutex = false;

            for (int i = 0; i < 100; i++) {
                AtomIni.SetValue(iniFilePath, "InteractiveLay", "Spacer" + i, "TestValue" + i);
            }
            
            /*
            for (int i = 0; i < 2000; i++) {
                string testValue = AtomIni.GetValue(iniFilePath, "InteractiveLay", "TestKeyTemp" + i, "TestValue" + i);
                //Console.WriteLine(testValue);
            }
            */

            //AtomIni.DeleteSection(iniFilePath, "Software\\String\\Nesting System\\InteractiveLay");
            
            Log.Information("FINISHED");
            
            
        }
    }
}
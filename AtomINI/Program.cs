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
        }
    }
}
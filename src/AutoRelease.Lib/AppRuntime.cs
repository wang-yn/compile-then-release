using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommandLine;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace AutoRelease
{
    public static class AppRuntime
    {
        public static void Initialize(string[] args)
        {
            InitLog();

            Parser.Default.ParseArguments<InitializerOption>(args)
                .WithParsed(options =>
                {
                    // todo:入参校验

                    SolutionWatcher watcher = new SolutionWatcher(options.SrcFolder, options.SiteRootFolder);
                    watcher.Start();
                });
        }

        private static void InitLog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
        }

    }

    class InitializerOption
    {
        [Option('s', "src", Required = true, HelpText = "源代码目录")]
        public string SrcFolder { get; set; }

        [Option('r', "root", Required = false, Default = "00根站点", HelpText = "站点根目录相对路径")]
        public string SiteRootFolder { get; set; }
    }
}

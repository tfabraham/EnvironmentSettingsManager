using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using System.Reflection;

namespace EnvSettingsManager
{
    internal class CommandLineParser
    {
        private List<string> errorMessages = new List<string>();
        ILogger logger;

        internal CommandLineParser(ILogger logger)
        {
            this.logger = logger;
        }

        public EnvSettingsManagerArguments ParseCommandLine(string[] args)
        {
            if (Parser.ParseHelp(args) || args.Length < 1)
            {
                DisplayLogo();
                DisplayUsage();
                return null;
            }

            // first arg (Action) is parsed here, then we decide which EnvSettingsManagerArguments instantiate
            // after removing that arg from the array
            ProgramAction action;
            try
            {
                action = (ProgramAction)Enum.Parse(typeof(ProgramAction), args[0], true);
            }
            catch (ArgumentException)
            {
                logger.LogError(42, "Invalid Action argument.");
                DisplayUsage();
                return null;
            }

            string[] remainingArgs = new string[args.Length - 1];
            if (args.Length > 1)
                Array.Copy(args, 1, remainingArgs, 0, args.Length - 1);
            // fake instance
            EnvSettingsManagerArguments parsedArgs = new EnvSettingsManagerArguments();
            string tname = "EnvSettingsManager." + action.ToString() + "ActionArguments";
            Type tsub = Type.GetType(tname);
            if (tsub != null)
            {
                parsedArgs = (EnvSettingsManagerArguments)Activator.CreateInstance(tsub);
            }
            else
            {
                throw new ArgumentException("Internal error: no class descendant from EnvSettingsManagerArguments matches ProgramAction values");
            }
            parsedArgs.action = action;

            // parse remaining arguments
            if (!Parser.ParseArguments(remainingArgs, parsedArgs, OnCommandLineParsingErrors))
            {
                DisplayLogo();
                DisplayErrors();
                DisplayUsage();
                return null;
            }
            if (!parsedArgs.nologo)
            {
                DisplayLogo();
            }
            return parsedArgs;
        }

        public void OnCommandLineParsingErrors(string message)
        {
            errorMessages.Add(message);
        }

        internal void DisplayLogo()
        {
            Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            ConsoleColor defaultConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Environment Settings Transposer {0}.{1}.{2}",
                assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
            Console.WriteLine("Copyright (C) 2007-10 Thomas F. Abraham, 2010 Giulio Vian.  All Rights Reserved.");

            Console.ForegroundColor = defaultConsoleColor;
        }

        internal void DisplayUsage()
        {
            string[] actionNames = Enum.GetNames(typeof(ProgramAction));
            string exeName = Assembly.GetExecutingAssembly().ManifestModule.Name;
            logger.LogInfo(string.Format(
                "Usage: {0} {1} [actionSpecificArguments*]"
                , exeName,
                string.Join("|", actionNames)));
            logger.LogInfo("");
            foreach (var name in actionNames)
            {
                Type t = Type.GetType("EnvSettingsManager." + name + "ActionArguments");
                if (t != null && t.IsSubclassOf(typeof(EnvSettingsManagerArguments)))
                {
                    logger.LogInfo(string.Format("{0} action valid arguments:", name));
                    logger.LogInfo(Parser.ArgumentsUsage(t));
                }
            }//for
        }

        internal void DisplayErrors()
        {
            foreach (var message in errorMessages)
            {
                logger.LogError(42, message);
            }
        }


    }
}

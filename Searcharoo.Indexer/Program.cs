using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Searcharoo.Common;
using Mono.GetOptions;           //v4
using PFitzsimons.ConsoleColour; //v6

#region Mono.GetOptions attributes to drive command line argument parsing
// Instructions to 'set up' Mono.GetOptions
// http://www.talios.com/command_line_processing_with_monogetoptions.htm

// Attributes visible in " -V"
[assembly: Mono.About("Searcharoo Indexer (Spider)")]
[assembly: Mono.Author("Craig.Dunn (at) ConceptDevelopment.net")]
[assembly: Mono.AdditionalInfo("Searcharoo.Indexer.exe spiders and catalogs data for the Searcharoo.Engine")]

// This is text that goes after " [options]" in help output - there is none for this program
[assembly: Mono.UsageComplement("")]

// Attributes visible in " --help"
// are defined in AssemblyInfo.cs (not here)
#endregion

namespace Searcharoo.Indexer
{
    /// <summary>
    /// Searcharoo INDEXER console application
    /// </summary>
    /// <remarks>
    /// Colored-coded output courtesy of
    /// http://www.codeproject.com/csharp/Console_Apps__Colour_Text/ConsoleColour_src.zip
    /// (Philip Fitzsimons)
    /// </remarks>
    class Program
    {
        private static CommandLinePreferences clip;

        static void Main(string[] args)
        {
            clip = new CommandLinePreferences();

            clip.ProcessArgs(args);

            if (clip.Verbosity > 0)
            {
                ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Green, true);
                Console.Write("Searcharoo.Indexer");
                ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Red, true);
                Console.WriteLine(" v0.3");
                ConsoleColour.SetForeGroundColour();
            }

            ConsoleWriteLine(1, "=======================");
            Spider spider = new Spider();

            spider.SpiderProgressEvent += new SpiderProgressEventHandler(OnProgressEvent);
            spider.SpiderProgressEvent += new SpiderProgressEventHandler(OnProgressLogEvent);

            string[] startPages = Preferences.StartPage.Split(new char[] { ',',';'});
           
            Uri[] uris = new Uri[startPages.Length];
            for (int i = 0; i < startPages.Length; i++)
            {
                uris[i] = new Uri(startPages[i]);
            }
            Catalog catalog = null;
            if (uris.Length == 1)
            {   // legacy behaviour, just for testing/comparison
                catalog = spider.BuildCatalog(new Uri(Preferences.StartPage));
            }
            else
            {   // multiple start Uris allowed
                catalog = spider.BuildCatalog(uris);
            }
           
            ConsoleWriteLine(1, "=======================");
#if DEBUG
            //System.Threading.Thread.Sleep(30 * 1000);    // 30 seconds
            ConsoleWriteLine(1, "Press <enter> to finish...");
            if (clip.Verbosity > 0) Console.Read();
#endif            
        }

        private static void ConsoleWriteLine(int level, string text)
        {
            switch (level)
            {
                case 2:
                    ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Grey);
                    break;
                case 3:
                    ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Yellow);
                    break;
                case 4:
                    ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Red);
                    break;
                case 5:
                    ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Magenta);
                    break;
                default:
                    ConsoleColour.SetForeGroundColour();
                    break;
            }
            if (level <= clip.Verbosity)
            {
                Console.WriteLine(text);
            }
        }

        /// <summary>
        /// Handle events generated by the Spider (mostly reporting on success/fail of page load/index)
        /// </summary>
        public static void OnProgressEvent(object source, ProgressEventArgs pea)
        {
            if (pea.Level <= clip.Verbosity)
            {
                switch (pea.Level)
                {
                    case 2:
                        ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Grey);
                        break;
                    case 3:
                        ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Yellow);
                        break;
                    case 4:
                        ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Red);
                        break;
                    case 5:
                        ConsoleColour.SetForeGroundColour(ConsoleColour.ForeGroundColour.Magenta);
                        break;
                    default:
                        ConsoleColour.SetForeGroundColour();
                        break;
                }
                Console.WriteLine(">{0} :: {1}", pea.Level, pea.Message);
            }
            
        }

        /// <summary>
        /// Log to disk events generated by the Spider (mostly reporting on success/fail of page load/index)
        /// </summary>
        public static void OnProgressLogEvent(object source, ProgressEventArgs pea)
        {
            //if (pea.Level < 3)
            //{
            //    Console.WriteLine(pea.Message + "<br>");
            //}
        }
    }
}

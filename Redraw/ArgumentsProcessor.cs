using NDesk.Options;
using System;
using GhostscriptSharp;
using GhostscriptSharp.Settings;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;

namespace Redraw
{
    public static class ArgumentsProcessor
    {
        private static OptionSet optionSet;
        private static bool showUsage;

        private static string inputPathParseResult;
        private static string outputPathParseResult;
        private static string widthParseResult;
        private static string heightParseResult;
        private static string firstPageParseResult;
        private static string lastPageParseResult;

        private static string inputPath;
        private static string outputPath;
        private static int width;
        private static int height;
        private static int firstPage;
        private static int lastPage;

        private static OptionSet OptionSet
        {
            get
            {
                if (optionSet == null)
                {
                    optionSet = CreateOptionSet();
                }

                return optionSet;
            }
        }

        public static RedrawConfig Process(string[] arguments)
        {
            try
            {
                OptionSet.Parse(arguments);

                if (!showUsage)
                {
                    ProcessOptions();
                    PostProcessOptions();
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                showUsage = true;
            }

            if (showUsage)
            {
                ShowUsage();
                return null;
            }

            return new RedrawConfig(inputPath, outputPath, firstPage, lastPage, width, height);
        }

        private static OptionSet CreateOptionSet()
        {
            if (optionSet != null)
            {
                throw new InvalidOperationException("OptionSet has already been created");
            }

            return new OptionSet()
            {
                { "i|input=",
                    "path to the PDF",
                    o => inputPathParseResult = o },
                { "o|output=",
                    "path to write resulting JPEG(s) to\n"
                    + "add %d somewhere to have each of the pages numbered\n"
                    + "DEFAULT: '<input>%d.jpg'",
                    o => outputPathParseResult = o },
                { "w|width=",
                    "target width of resulting JPEG(s)",
                    o => widthParseResult = o },
                { "h|height=",
                    "target height of resulting JPEG(s)",
                    o => heightParseResult = o },
                { "f|first=",
                    "first page to render\n"
                    + "DEFAULT: first page of PDF",
                    o => firstPageParseResult = o },
                { "l|last=",
                    "last page to render\n"
                    + "DEFAULT: last page of PDF",
                    o => lastPageParseResult = o },
                { "?|help",
                    "show this message and exit",
                    o => showUsage = o != null }
            };
        }

        private static void ProcessOptions()
        {
            new OptionProcessor(inputPathParseResult, "input")
                .AssertIsSet()
                .Process(p => inputPath = p);

            new OptionProcessor(outputPathParseResult, "output")
                .AddDefault(inputPath + "%d.jpg")
                .Process(p => outputPath = p);

            new OptionProcessor(widthParseResult, "width")
                .AssertIsSet()
                .ProcessAsInt(p => width = p);

            new OptionProcessor(heightParseResult, "height")
                .AssertIsSet()
                .ProcessAsInt(p => height = p);

            new OptionProcessor(firstPageParseResult, "first")
                .AddDefault("1")
                .ProcessAsInt(p => firstPage = p);

            new OptionProcessor(lastPageParseResult, "last")
                .ProcessAsInt(p => lastPage = p);
        }

        private static void PostProcessOptions()
        {
            // Size is divided by 10 because for some reason GhostscriptSharp multiplies it by 10
            width = width >= 10
                ? width / 10
                : width;

            height = height >= 10
                ? height / 10
                : height;
        }

        private static void ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Redraw " + GetVersionInfo().FileVersion);
            Console.WriteLine("Renders PDF pages to JPEGs");
            Console.WriteLine(GetVersionInfo().LegalCopyright);
            Console.WriteLine();
            Console.WriteLine("Usage: Redraw -i <input> -w <width> -h <height> [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            OptionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("A note on resolution:");
            Console.WriteLine("width and height of the resulting JPEG(s) will be adjusted to match the PDF's aspect ratio");
        }

        private static FileVersionInfo GetVersionInfo()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        }
    }
}

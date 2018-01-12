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
                    "the path to the file to be rendered",
                    o => inputPathParseResult = o },
                { "o|output=",
                    "the path to write the resulting image to\n"
                    + "add %d somewhere to have each of the pages numbered\n"
                    + "DEFAULT: '<input>%d'",
                    o => outputPathParseResult = o },
                { "w|width=",
                    "the width of the resulting image",
                    o => widthParseResult = o },
                { "h|height=",
                    "the height of the resulting image",
                    o => heightParseResult = o },
                { "f|first=",
                    "the first page to render\n"
                    + "if <last> is not set, only this page will be rendered\n"
                    + "DEFAULT: '1'",
                    o => firstPageParseResult = o },
                { "l|last=",
                    "the last page to render",
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
                .AddDefault(inputPath + "%d")
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
            Console.WriteLine("Renders pdf pages to images");
            Console.WriteLine(GetVersionInfo().LegalCopyright);
            Console.WriteLine();
            Console.WriteLine("Usage: Redraw -i <input> -w <width> -h <height> [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            OptionSet.WriteOptionDescriptions(Console.Out);
        }

        private static FileVersionInfo GetVersionInfo()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        }
    }
}

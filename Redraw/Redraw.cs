using GhostscriptSharp;
using System;
using System.IO;

namespace Redraw
{
    class Redraw
    {
        static int Main(string[] arguments)
        {
            try
            {
                return Run(arguments);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        private static int Run(string[] arguments)
        {
            RedrawConfig config = ArgumentsProcessor.Process(arguments);

            if (config == null)
            {
                return 1;
            }

            FileInfo inputFile = new FileInfo(config.InputPath);

            if (!inputFile.Exists)
            {
                Console.WriteLine("Specified input file does not exist");
                return 1;
            }

            DirectoryInfo outputDir = new DirectoryInfo(Path.GetDirectoryName(config.OutputPath));

            if (!outputDir.Exists)
            {
                outputDir.Create();
            }
            
            GhostscriptWrapper.GeneratePageThumbs(inputFile.FullName, config.OutputPath, config.FirstPage, config.LastPage, config.Width, config.Height);

            return 0;
        }
    }
}

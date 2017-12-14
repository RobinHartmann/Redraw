using GhostscriptSharp;

namespace Redraw
{
    public class RedrawConfig
    {
        public RedrawConfig(string inputPath, string outputPath, GhostscriptSettings ghostscriptSettings)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
            GhostscriptSettings = ghostscriptSettings;
        }

        public string InputPath
        {
            get;
            private set;
        }

        public string OutputPath
        {
            get;
            private set;
        }

        public GhostscriptSettings GhostscriptSettings
        {
            get;
            private set;
        }
    }
}

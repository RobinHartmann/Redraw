using GhostscriptSharp;

namespace Redraw
{
    public class RedrawConfig
    {
        public RedrawConfig(string inputPath, string outputPath, int firstPage, int lastPage, int width, int height)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
            FirstPage = firstPage;
            LastPage = lastPage;
            Width = width;
            Height = height;
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

        public int FirstPage
        {
            get;
            private set;
        }

        public int LastPage
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }
    }
}

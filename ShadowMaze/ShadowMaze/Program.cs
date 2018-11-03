namespace ShadowMaze
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ShadowMaze game = new ShadowMaze())
            {
                game.Run();
            }
        }
    }
#endif
}
using System.Runtime.InteropServices;

namespace NET_LAb_1
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        [STAThread]
        static void Main()
        {
            if (Config.IsDebugMode)
                AllocConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            Application.Run(new MainMenu());
        }
    }
}
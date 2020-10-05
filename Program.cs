using System;
using NLog.Web;
using System.IO;

namespace MediaLibraryLab
{
    class Program
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            Console.WriteLine("Hello World!");
            
            logger.Info("Program ended");
        }
    }
}

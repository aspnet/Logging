using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HelloLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new LoggerFactory();
            factory.AddConsole();
            var logger = factory.CreateLogger<Program>();
            logger.LogInformation("Hello logging");
        }
    }
}

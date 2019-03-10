using System;
using System.Threading;
using Serilog;

namespace LogPipelineTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Http("http://localhost:19501")
                .CreateLogger();

            var user = User.CreateRandom();
            log.Information("{@User} registered", user);

            Log.CloseAndFlush();
            Thread.Sleep(2000);
        }
    }

    internal class User
    {
        public static User CreateRandom()
        {
            return new User()
            {
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last()
            };
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

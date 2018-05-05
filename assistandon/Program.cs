using System;
using Topshelf;

namespace assistandon
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<Logic>(s =>
                {
                    s.ConstructUsing(name => new Logic());
                    s.WhenStarted(ml => ml.Start());
                    s.WhenStopped(ml => ml.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("");
                x.SetDisplayName("");
                x.SetServiceName("Assistandon");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }

}

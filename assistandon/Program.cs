using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Topshelf;
using Mastonet;
using Mastonet.Entities;
using System.Text.RegularExpressions;

namespace assistandon
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<MainLogic>(s =>
                {
                    s.ConstructUsing(name => new MainLogic());
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

    public class MainLogic
    {
        public void Start() { logic(); }
        public void Stop() { }



        static void logic()
        {
            // Mastodon 認証関連
            var appRegistration = new AppRegistration
            {
                Instance = ConfigurationManager.AppSettings["instanceUrl"],
                ClientId = ConfigurationManager.AppSettings["clientID"],
                ClientSecret = ConfigurationManager.AppSettings["clientSecret"],
                Scope = Scope.Read
            };
            var authClient = new AuthenticationClient(appRegistration);
            var auth = authClient.ConnectWithPassword(ConfigurationManager.AppSettings["loginId"], ConfigurationManager.AppSettings["loginPass"]).Result;
            var client = new MastodonClient(appRegistration, auth);
            Console.WriteLine("mastodon login ok");
            
            LTL_stream(client);
        }

        static async void LTL_stream(MastodonClient client)
        {
            Console.WriteLine("LTL Connect");

            //LTLストリーム取得設定(mastonet改造拡張機能)
            var ltlStreaming = client.GetLocalStreaming();

            //htmlタグ除去
            var rejectHtmlTagReg = new Regex("<.*?>");

            ltlStreaming.OnUpdate += (sender, e) =>
            {
                Console.WriteLine("update:"+e.Status.Account.DisplayName+":"+e.Status.Content);
            };
            await ltlStreaming.Start();
        }
    }
}

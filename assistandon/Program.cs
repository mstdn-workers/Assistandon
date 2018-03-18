using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Topshelf;
using Mastonet;
using Mastonet.Entities;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

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

    class Logic
    {
        public void Start() { MainLogic(); }
        public void Stop() { }

        private MastodonClient client;

        public Dictionary<string, List<string>> WaitingBoard = new Dictionary<string, List<string>>();


        void MainLogic()
        {
            var appRegistration = AppRegistrateLogic();
            var auth = AuthLogic();
            this.client = new MastodonClient(appRegistration, auth);

            LTL_stream();
        }


        AppRegistration AppRegistrateLogic()
        {
            var fileName = @".\AppRegistration.xml";
            var appreg = new AppRegistration();

            if (File.Exists(fileName))
            {
                Console.WriteLine($"\"{fileName}\"を読み込みます。");
                var serializer = new XmlSerializer(typeof(AppRegistration));
                using(var sr = new StreamReader(fileName))
                {
                    appreg = (AppRegistration)serializer.Deserialize(sr);
                }
            }
            else
            {
                Console.WriteLine($"\"{fileName}\"が存在しません。作成します。設定値を編集して保存してください。");
                appreg = new AppRegistration
                {
                    Instance = "Instance is Here! (ex: example.com)",
                    ClientId = "Client_id is Here!",
                    ClientSecret = "Client_secret is Here!",
                    Scope = Scope.Follow | Scope.Read | Scope.Write
                };
                var serializer = new XmlSerializer(typeof(AppRegistration));
                using(var sw = new StreamWriter(fileName))
                {
                    serializer.Serialize(sw, appreg);
                }
            }
            return appreg;
        }

        Auth AuthLogic()
        {

            var fileName = @".\Auth.xml";
            var auth = new Auth();

            if (File.Exists(fileName))
            {
                Console.WriteLine($"\"{fileName}\"を読み込みます。");
                var serializer = new XmlSerializer(typeof(Auth));
                var sr = new StreamReader(fileName);
                auth = (Auth)serializer.Deserialize(sr);
            }
            else
            {
                auth.AccessToken = "Access_token is Here!";
                Console.WriteLine($"\"{fileName}\"が存在しません。作成します。設定値を編集して保存してください。");
                var serializer = new XmlSerializer(typeof(Auth));
                var sw = new StreamWriter(fileName);
                serializer.Serialize(sw, auth);
            }
            return auth;
        }

        async void LTL_stream()
        {
            Console.WriteLine("LTL Connect");

            //LTLストリーム取得設定(mastonet改造拡張機能)
            var ltlStreaming = this.client.GetLocalStreaming();

            //htmlタグ除去
            var rejectHtmlTagReg = new Regex("<.*?>");
            

            ltlStreaming.OnUpdate += (sender, e) =>
            {
                var content = rejectHtmlTagReg.Replace(e.Status.Content, "");
                Console.WriteLine("update:" + e.Status.Account.Id + ":" + content);

                this.CalledMe(content);
                this.WaitCheckLogic(e.Status.Account.UserName);
            };
            await ltlStreaming.Start();
        }

        void CalledMe(string text)
        {
            var pattern = "^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*$";
            if (Regex.IsMatch(text, pattern))
            {
                this.client.PostStatus("呼んだ？", Visibility.Public);
            }
        }
        
        void WaitCheckLogic(string userName)
        {
            try
            {
                var users = WaitingBoard[userName];
                foreach(var user in users)
                {
                    client.PostStatus($"@{user} {userName}さん来たよ～", Visibility.Direct);
                }
                this.WaitingBoard.Remove(userName);
            }
            catch(Exception e)
            {
                Console.WriteLine($"WaitCheckLogic {userName}: {e.Message}");
            }
        }

        void WaitersCame()
        {
            client.PostStatus("きた", Visibility.Public);
        }

    }
}

﻿using System;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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

        // Mastodon clients
        private MastodonClient client;

        // サービス開始時刻
        private DateTime serviceStartedTime = DateTime.Now;
        // 最終ハートビート確認時刻
        private DateTime lastHeartBeatTime = DateTime.Now;

        // earthquake userId
        private long earthquakeUserId = long.Parse(ConfigurationManager.AppSettings["earthquakeUserId"]);

        private Dictionary<string, List<string>> WaitingBoard = new Dictionary<string, List<string>>();

        private Dictionary<long, DateTime> calledUsers = new Dictionary<long, DateTime>();

        private DateTime calledMeDateTime = new DateTime();
        private DateTime quakeCheckDateTime = new DateTime();


        // MainLogic
        async Task MainLogic()
        {
            // htmlタグ除去
            var rejectHtmlTagReg = new Regex("<.*?>");

            // MastodonClient初期化
            this.client = new MastodonClient(AppRegistrateLogic(), AuthLogic());
            
            //LTLストリーム取得設定(mastonet改造拡張機能)
            var ltlStreaming = this.client.GetLocalStreaming();
            // LTLアップデート時処理
            ltlStreaming.OnUpdate += (sender, e) =>
            {
                var content = rejectHtmlTagReg.Replace(e.Status.Content, "");
                Console.WriteLine($"update from {e.Status.Account.UserName}: {content}");
                this.LocalUpdateBranch(e);
            };
            // ハートビート受信処理
            ltlStreaming.OnHeartbeat += (sender, e) =>
            {
                this.lastHeartBeatTime = DateTime.Now;
            };
            
            // mentionのところ
            var userStreaming = this.client.GetUserStreaming();
            userStreaming.OnNotification += (sender, e) =>
            {
                var content = rejectHtmlTagReg.Replace(e.Notification.Status.Content, "");
                Console.WriteLine($"notification from {e.Notification.Account.UserName}: {content}");
                this.UserNotificationBranch(e);
            };

            

            // awaitしない！
            Task.Run(() => ltlStreaming.Start());
            Task.Run(() => userStreaming.Start());
            Task.Run(() => this.HeartBeatCheck());
        }


        // 処理分割
        void LocalUpdateBranch(StreamUpdateEventArgs e)
        {
            // htmlタグ除去
            var rejectHtmlTagReg = new Regex("<.*?>");
            var content = rejectHtmlTagReg.Replace(e.Status.Content, "");

            if (Regex.IsMatch(content, RegexStringSet.QuakeCheckPattern) && DateTime.Now.CompareTo(this.quakeCheckDateTime + new TimeSpan(0, 15, 0)) == 1)
                this.QuakeCheck();
            else if (Regex.IsMatch(content, RegexStringSet.WhatTimePattern) && DateTime.Now.CompareTo(this.calledMeDateTime + new TimeSpan(0, 5, 0)) == 1)
                this.WhatTime();
            else if (Regex.IsMatch(content, RegexStringSet.CallMePattern) && DateTime.Now.CompareTo(this.calledMeDateTime + new TimeSpan(0, 5, 0)) == 1)
                this.CalledMe(content);
        }

        void UserNotificationBranch(StreamNotificationEventArgs e)
        {
            // htmlタグ除去
            var rejectHtmlTagReg = new Regex("<.*?>");
            var content = rejectHtmlTagReg.Replace(e.Notification.Status.Content, "");

            if (Regex.IsMatch(content, RegexStringSet.AdminCommandExecPattern) && e.Notification.Account.Id == long.Parse(ConfigurationManager.AppSettings["adminId"]))
                this.AdminCommand(content, e);
            else if (Regex.IsMatch(content, RegexStringSet.DirectQuakeCheckPattern))
                this.QuakeCheck(e.Notification.Account.UserName);
        }


        // 定期実行処理
        void Cycle()
        {
            while (true)
            {
                Thread.Sleep(5000);

                this.HeartBeatCheck();
            }
        }


        // 各種関数
        void AdminCommand(string content, StreamNotificationEventArgs e)
        {
            Console.WriteLine("<<--  Admin Command START.  -->>");
            Console.WriteLine($"Receive: {content}");
            Console.WriteLine("  ----------------------------  ");
            if (Regex.IsMatch(content, @"^.*(admincmd)$"))
            {
                // 死活確認
                var tootText = $"@{e.Notification.Account.UserName} I'm up.";
                this.client.PostStatus(tootText, Visibility.Direct, e.Notification.Status.Id);
                Console.WriteLine($"Toot: {tootText}");
            }
            else if (Regex.IsMatch(content, @"^.*(admincmd) (restart)$"))
            {
                // サービス再起動
                Console.WriteLine("アプリケーションを再起動します。");
                this.ServiceRestart();

            }
            else if (Regex.IsMatch(content, @"^.*(admincmd) (startdate)$"))
            {
                // サービス開始時刻
                var tootText = $"@{e.Notification.Account.UserName} {this.serviceStartedTime}";
                this.client.PostStatus(tootText, Visibility.Direct, e.Notification.Status.Id);
                Console.WriteLine($"Toot: {tootText}");
            }
            else if (Regex.IsMatch(content, @"^.*(admincmd) (yure)$"))
            {
                // 地震情報
                this.QuakeCheck();
            }
                Console.WriteLine("<<--  Admin Command STOP.  -->>");
        }

        void QuakeCheck(string userName="")
        {
            this.quakeCheckDateTime = DateTime.Now;
            
            var options = new ArrayOptions();
            options.Limit = 1;
            try
            {
                var nervstatuses = this.client.GetAccountStatuses(this.earthquakeUserId, options).Result;
                // htmlタグ除去
                var rejectHtmlTagReg = new Regex("<.*?>");
                var content = rejectHtmlTagReg.Replace(nervstatuses[0].Content, string.Empty);
                // var url = nervstatuses[0].Url;
                var url = "https://unnerv.jp/";

                var jisinReg = new Regex(@"((\d{4})年(\d{1,})月(\d{1,})日)(】)((\d{1,})時(\d{1,})分)(頃、)(.*)(?:を震源とする)(.*)(?:最大震度)(.*?)(?:を)(.*)(?:で観測して)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var m = jisinReg.Match(content);

                var datestr = m.Groups[1].Value;
                var timestr = m.Groups[6].Value;
                var kansokuchi = m.Groups[13].Value;
                var shindo = m.Groups[12].Value;
                var shingen = m.Groups[10].Value;

                var year = int.Parse(m.Groups[2].Value);
                var month = int.Parse(m.Groups[3].Value);
                var day = int.Parse(m.Groups[4].Value);
                var hour = int.Parse(m.Groups[7].Value);
                var min = int.Parse(m.Groups[8].Value);

                var quakeTime = new DateTime(year, month, day, hour, min, 0);
                var elapsedTime = DateTime.Now - quakeTime;

                var tootText = string.Empty;

                if ((elapsedTime < new TimeSpan(0, 15, 0)) || (elapsedTime < new TimeSpan(0, 30, 0) && shindo != "1"))
                    tootText = $"{timestr}頃に{kansokuchi}で震度{shindo}の地震があったらしいよ。震源は{shingen}みたいだね。詳細はここに書いてあったよ。{url}";
                else if (shindo == "1")
                    tootText = $"私の胸は揺れなかった。。。";
                else if (elapsedTime < new TimeSpan(0, 30, 0))
                    tootText = $"いつの話をしているの？直近の地震は{timestr}頃に{kansokuchi}で観測した震度{shindo}だよ？";
                else if (elapsedTime < new TimeSpan(1, 0, 0))
                    tootText = $"いつの話をしているの？";
                else if (elapsedTime > new TimeSpan(1, 0, 0, 0))
                    tootText = $"今日は揺れてないよ。";

                if (userName == "")
                {
                    // 通常処理
                    this.client.PostStatus(tootText, Visibility.Public);
                }
                else
                {
                    // ユーザ指定時処理
                    this.client.PostStatus($"@{userName} {tootText}", Visibility.Direct);
                }

                Console.WriteLine(tootText);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        void WhatTime()
        {
            this.client.PostStatus($"{DateTime.Now}", Visibility.Public);
        }

        void CalledMe(string content)
        {
            this.client.PostStatus("呼んだ？", Visibility.Public);
            this.calledMeDateTime = DateTime.Now;
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

        void HeartBeatCheck()
        {
            if(DateTime.Now - this.lastHeartBeatTime > new TimeSpan(0, 0, 45))
            {
                try
                {
                    this.client.PostStatus("ストリーム途切れてるみたいだからサービス再起動するね。",Visibility.Public );
                    ServiceRestart();
                }
                catch
                {
                    ServiceRestart();
                }
            }
            else if (DateTime.Now - this.lastHeartBeatTime > new TimeSpan(0, 0, 20))
            {
                try
                {
                    this.client.PostStatus("ストリーム途切れてるっぽい？", Visibility.Public);
                }
                catch
                {
                    ServiceRestart();
                }
            }
            
        }

        void ServiceRestart()
        {
            Environment.Exit(-1);
        }

        // なぜか動かない
        void ShutdownToot()
        {
            this.client.PostStatus("Yuki, サービスを停止します.", Visibility.Public);
        }

        AppRegistration AppRegistrateLogic(string fileName = @".\AppRegistration.xml")
        {
            var appreg = new AppRegistration();

            if (File.Exists(fileName))
            {
                Console.WriteLine($"\"{fileName}\"を読み込みます。");
                var serializer = new XmlSerializer(typeof(AppRegistration));
                using (var sr = new StreamReader(fileName))
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
                using (var sw = new StreamWriter(fileName))
                {
                    serializer.Serialize(sw, appreg);
                }
            }
            Console.WriteLine($"Instance: {appreg.Instance}");
            Console.WriteLine($"ClientID: {appreg.ClientId}");
            Console.WriteLine($"ClientSecret: {appreg.ClientSecret}");


            return appreg;
        }

        Auth AuthLogic(string fileName = @".\Auth.xml")
        {
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

            Console.WriteLine($"AccessToken: {auth.AccessToken}");

            return auth;
        }
    }
}

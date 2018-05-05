using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assistandon
{
    class RegexStringSet
    {
        public static string CallMePattern = @"^(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*$";
        public static string QuakeCheckPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*((揺|ゆ)れた).*$";
        public static string DirectQuakeCheckPattern = @"^.*((揺|ゆ)れた).*$";
        public static string EewPattern = @"(最終報)(.*)((\d{4})年(\d{1,})月(\d{1,})日)】((\d{1,})時(\d{1,})分)(頃、)(.*)(?:を震源とする)(.*)(?:最大震度)(.*?)(?:と推定)";
        public static string WhatTimePattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*((今|いま)(何時|なんじ)).*$";
        public static string YukiOutPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*(あうと|アウト|ｱｳﾄ).*$";
        public static string AdminCommandExecPattern = @"^(.*)(admincmd)(.*)$";
        public static string NewComerPattern = @"^(初めまして)@(\w*).(.*?)さん#お久bot$";
        public static string SetNickName = @"(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)([^A-Za-z_0-9]*)([A-Za-z_0-9]*).*(は|を)(.*)(って|と)(よんで|読んで|呼んで)";
        public static string NotSetPattern = @"(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)([^A-Za-z_0-9]*)([A-Za-z_0-9]*).*(は|を)((ゆき|ユキ|悠希)|(よんで|読んで|呼んで)|(ﾃﾞﾃﾞｰﾝ)|(デデーン)|(姉)|(\dd\d)|(おおおおお*)|(@)|(の天気)|(の道路)|(、まおー城爆破))";
        public static string SetReverseName = @"^.*「(.*)」.*?(\w*?)(?!\w).*$";
        public static string MaohBombPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)、まおー城爆破.*$";
        public static string WeatherPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)、(.*?)の天気.*$";
        public static string RoadPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)、(.*?)の道路.*$";
        public static string RecordEvidencePattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)、(エビデンス|えびでんす|えびだんす|evi).*$";
        public static string KiribanPattern = @"^(@\w*)(投稿数が) (\d*) (でキリ番になりました！)$";
        public static string NullpoPattern = @"((ぬ|ヌ|ﾇ)(()| |　|¥n)*(る|ル|ﾙ)(()| |　)*(ぽ|ポ|ﾎﾟ))|((ぽ|ポ|ﾎﾟ)(()| |　)*(る|ル|ﾙ)(()| |　)*(ぬ|ヌ|ﾇ))";
        public static string MatsuyaPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」)、(.*)松屋.*$";
    }
}

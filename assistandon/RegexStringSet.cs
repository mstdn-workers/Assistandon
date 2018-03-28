using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assistandon
{
    class RegexStringSet
    {
        public static string CallMePattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*$";
        public static string QuakeCheckPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*((揺|ゆ)れた).*$";
        public static string DirectQuakeCheckPattern = @"^.*((揺|ゆ)れた).*$";
        public static string EewPattern = @"(最終報)(.*)((\d{1,})時(\d{1,})分)(頃、)(.*)(?:を震源とする)(.*)(?:最大震度)(.*?)(?:と推定)";
        public static string WhatTimePattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*((今|いま)(何時|なんじ)).*$";
        public static string YukiOutPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*(あうと|アウト|ｱｳﾄ).*$";
        public static string AdminCommandExecPattern = @"^(.*)(admincmd)(.*)$";
        public static string NewComerPattern = @"^(初めまして)@(\w*).(.*?)さん#お久bot$";
        public static string SetNickName = @".*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).?(\w*).*?(は|を)(.*)(って|と)(よんで|読んで|呼んで)";
    }
}

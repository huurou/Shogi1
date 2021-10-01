using System;
using System.Collections.Generic;

namespace Shogi1.Infrastructure.Games
{
    /// <summary>
    /// 対局情報
    /// </summary>
    internal record GameInfo(DateTime Start, DateTime End,
                             string Black, string White, List<MoveInfo> MoveInfos);

    internal record MoveInfo(MoveType Type, int Turns,
                             Position From, Position To, Piece Piece,
                             TimeSpan Spent, TimeSpan TotalSpent);

    internal record Position(int X, int Y)
    {
        public override string ToString()
        {
            var x = X switch
            {
                1 => "１",
                2 => "２",
                3 => "３",
                4 => "４",
                5 => "５",
                6 => "６",
                7 => "７",
                8 => "８",
                9 => "９",
                _ => throw new InvalidOperationException(),
            };
            var y = Y switch
            {
                1 => "一",
                2 => "二",
                3 => "三",
                4 => "四",
                5 => "五",
                6 => "六",
                7 => "七",
                8 => "八",
                9 => "九",
                _ => throw new InvalidOperationException(),
            };
            return $"{x}{y}";
        }
    }

    internal enum MoveType
    {
        Normal, Promote, Drop, Suspend, Resign,
    }

    internal enum Piece
    {
        None, 玉, 飛, 龍, 角, 馬,
        金, 銀, 成銀, 桂, 成桂,
        香, 成香, 歩, と,
    }
}
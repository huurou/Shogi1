using System;
using System.Collections.Generic;

namespace Shogi1.Infrastructure.Games
{
    /// <summary>
    /// 対局情報
    /// </summary>
    internal class GameInfo
    {
        internal DateTime StartDateTime { get; init; }
        internal DateTime EndDateTime { get; init; }
        internal string Black { get; init; } = "";
        internal string White { get; init; } = "";
        internal List<MoveInfo> MoveInfos { get; init; } = new();
    }

    internal class MoveInfo
    {
        internal MoveType Type { get; init; }
        internal int Turns { get; init; }
        internal string To { get; init; } = "";
        internal string Piece { get; init; } = "";
        internal string Modifier { get; init; } = "";
        internal string From { get; init; } = "";
        internal TimeSpan SpentTime { get; init; }
        internal TimeSpan TotalSpentTime { get; init; }
    }

    internal enum MoveType
    {
        Normal,
        Resign,
    }
}
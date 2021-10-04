using Shogi1.Domain.Model.Moves;
using System;
using System.Collections.Generic;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.TTs
{
    internal static class TT
    {
        private const int POW = 20;
        private const int SIZE = 1 << POW;
        private static readonly TTEntry[] tt_ = new TTEntry[SIZE];

        internal static void Save(ulong hash, TTEntry entry) => tt_[hash & (SIZE - 1)] = entry;

        internal static bool TryFetch(ulong hash, out TTEntry entry)
        {
            entry = tt_[hash & (SIZE - 1)];
            return entry is not null;
        }
    }

    internal class TTEntry
    {
        internal List<MoveBase> Moves { get; }

        public TTEntry(List<MoveBase> moves) => Moves = moves;
    }

    internal static class Zobrist
    {
        /// <summary>
        /// Rand64のseed
        /// </summary>
        private static ulong s_;

        /// <summary>
        /// 駒pcが盤上sqに配置されているときのZobrist Key
        /// </summary>
        internal static ulong[,] SquarePiece { get; } = new ulong[BOARD_POW, (int)PieceLast];

        /// <summary>
        /// c側の手駒prが一枚増えるごとにこれを加算するZobristKey
        /// </summary>
        internal static ulong[,] Hand { get; } = new ulong[2, (int)PieceLast];

        static Zobrist()
        {
            s_ = 20211005UL;
            for (var i = 0; i < SquarePiece.GetLength(0); i++)
            {
                for (var j = 1; j < SquarePiece.GetLength(1); j++)
                {
                    SquarePiece[i, j] = Rand64();
                }
            }
            for (var i = 0; i < Hand.GetLength(0); i++)
            {
                for (var j = 0; j < Hand.GetLength(1); j++)
                {
                    Hand[i, j] = Rand64();
                }
            }
        }

        private static ulong Rand64()
        {
            s_ ^= s_ >> 12;
            s_ ^= s_ << 25;
            s_ ^= s_ >> 27;
            return s_ * 2685821657736338717UL;
        }
    }
}
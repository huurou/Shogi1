using Shogi1.Domain.Model.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.AIs.Evaluators
{
    internal class EffectValueEvaluator : IEvaluator
    {
        private readonly List<int> playerEffectValues_ = Enumerable.Range(0, 9).Select(i => 68 * 1024 / (i + 1)).ToList();
        private readonly List<int> opponentEffectValues_ = Enumerable.Range(0, 9).Select(i => 96 * 1024 / (i + 1)).ToList();

        public int Evaluate(Board board)
            => board.IsChackMate
                ? board.Teban ? -99999999 : 99999999
                : EffectValue(board);

        private int EffectValue(Board board)
        {
            var res = 0;
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 1; x <= 9; x++)
                {
                    var p = new Position(x, y);
                    var (eb, ew) = board.Effects(p);
                    var (db, dw) = (Dist(p, new Position(Array.IndexOf(board.Pieces, KingB))),
                                    Dist(p, new Position(Array.IndexOf(board.Pieces, KingW))));
                    res += (eb * playerEffectValues_[db] - ew * opponentEffectValues_[db]
                        - ew * playerEffectValues_[dw] + eb * opponentEffectValues_[dw]) / 1024;
                }
            }
            return res;
        }

        /// <summary>
        /// ある升から升への距離を求める
        /// 筋と段でたくさん離れている方の数をその距離とする
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static int Dist(Position p1, Position p2)
            => Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
    }
}
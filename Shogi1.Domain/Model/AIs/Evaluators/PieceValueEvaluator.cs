using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.AIs.Evaluators
{
    internal class PieceValueEvaluator : IEvaluator
    {
        private readonly Random random_ = new();

        private readonly int[] pieceValues_ = new int[]
        {
            0, 0, 990, 1395, 855, 945, 540, 495, 540, 405, 540, 315, 540, 90, 540,
            0, -990, -1395, -855, -945, -540, -495, -540, -405, -540, -315, -540, -90, -540,
        };

        public int Evaluate(Board board)
            => board.IsChackMate
                ? board.Teban ? B_CHECKMATE : W_CHECKMATE
                : PieceValue(board);

        private int PieceValue(Board board)
            => board.Pieces.Where(x => x.IsPiece()).Sum(x => pieceValues_[(int)x]) * 920 / 1024
                + board.HandsBlack.Concat(board.HandsWhite).Where(x => x.IsPiece()).Sum(x => pieceValues_[(int)x])
                + random_.Next(31) - 15;
    }

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
                    var (db, dw) = (Dist(p, new Position(Array.IndexOf(board.Pieces, 王B))),
                                    Dist(p, new Position(Array.IndexOf(board.Pieces, 王W))));
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

    internal class PieceValueAndEffectEvaluator : IEvaluator
    {
        private readonly int[] pieceValues_ = new int[]
        {
            0, 0, 990, 1395, 855, 945, 540, 495, 540, 405, 540, 315, 540, 90, 540,
            0, -990, -1395, -855, -945, -540, -495, -540, -405, -540, -315, -540, -90, -540,
        };

        private readonly List<int> playerEffectValues_ = Enumerable.Range(0, 9).Select(i => 85 * 1024 / (i + 1)).ToList();
        private readonly List<int> opponentEffectValues_ = Enumerable.Range(0, 9).Select(i => 98 * 1024 / (i + 1)).ToList();

        public int Evaluate(Board board)
            => board.IsChackMate
                ? board.Teban ? -99999999 : 99999999
                : PieceAndEffectValue(board);

        private int PieceAndEffectValue(Board board)
        {
            var res = 0;
            for (var y = 1; y <= 9; y++)
            {
                for (var x = 1; x <= 9; x++)
                {
                    var p = new Position(x, y);
                    var (eb, ew) = board.Effects(p);
                    var (db, dw) = (Dist(p, new Position(Array.IndexOf(board.Pieces, 王B))),
                                    Dist(p, new Position(Array.IndexOf(board.Pieces, 王W))));
                    res += (eb * playerEffectValues_[db] - ew * opponentEffectValues_[db]
                        - ew * playerEffectValues_[dw] + eb * opponentEffectValues_[dw]) / 1024;
                    var piece = board.Pieces[p];
                    if (piece.IsEmpty()) continue;
                    res += pieceValues_[(int)piece] * 920 / 1024;
                }
            }
            return res + board.HandsBlack.Concat(board.HandsWhite).Where(x => x.IsPiece()).Sum(x => pieceValues_[(int)x]);
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
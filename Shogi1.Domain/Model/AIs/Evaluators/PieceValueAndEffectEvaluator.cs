using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.AIs.Evaluators
{
    internal class PieceValueAndEffectEvaluator : IEvaluator
    {
        // 利きの数の閾値
        private const int EFFECT_TRESHOLD = 4;

        private readonly Random random_ = new();

        private readonly int[] pieceValues_ = new int[]
        {
            0, 0, 990, 1395, 855, 945, 540, 495, 540, 405, 540, 315, 540, 90, 540,
            0, -990, -1395, -855, -945, -540, -495, -540, -405, -540, -315, -540, -90, -540,
        };

        // 自玉距離に応じたある升の利きの価値。
        private readonly List<int> ourEffectValues_ = Enumerable.Range(0, 9).Select(d => 85 * 1024 / (d + 1)).ToList();

        // 相手玉の距離に応じたある升の利きの価値。
        private readonly List<int> theirEffectValues_ = Enumerable.Range(0, 9).Select(d => 98 * 1024 / (d + 1)).ToList();

        // 利きが一つの升に複数あるときの価値
        // 1024倍した値を格納する。
        // optimizerの答えは、{ 0 , 1024/* == 1.0 */ , 1800, 2300 , 2900,3500,3900,4300,4650,5000,5300 }
        //   6365 - pow(0.8525,m-1)*5341 　みたいな感じ？
        private readonly int[] multiEffectValue_ = new int[] { 0, 1024, 1800, 2300, 2900, 3500, 3900, 4300, 4650, 5000, 5300 };

        // 利きを評価するテーブル
        //    [自玉の位置][対象となる升][利きの数(0～3)]
        private readonly int[,,] ourEffectTable_ = new int[BOARD_POW, BOARD_POW, EFFECT_TRESHOLD + 1];

        //    [相手玉の位置][対象となる升][利きの数(0～3)]
        private readonly int[,,] theirEffectTable_ = new int[BOARD_POW, BOARD_POW, EFFECT_TRESHOLD + 1];

        // 利きの価値を合算した値を求めるテーブル
        // [先手玉,後手玉,対象升,先手の利き,後手の利き]
        private readonly int[,,,,] effectTable_ = new int[BOARD_POW, BOARD_POW, BOARD_POW, EFFECT_TRESHOLD + 1, EFFECT_TRESHOLD + 1];

        internal PieceValueAndEffectEvaluator()
        {
            for (var sq1 = 0; sq1 < BOARD_POW; sq1++)
            {
                for (var sq2 = 0; sq2 < BOARD_POW; sq2++)
                {
                    for (var m = 0; m < EFFECT_TRESHOLD + 1; m++)
                    {
                        var d = Dist(new(sq1), new(sq2));
                        // 利きには、王様からの距離に反比例する価値がある。
                        ourEffectTable_[sq1, sq2, m] = multiEffectValue_[m] * ourEffectValues_[d] / (1024 * 1024);
                        theirEffectTable_[sq1, sq2, m] = multiEffectValue_[m] * theirEffectValues_[d] / (1024 * 1024);
                    }
                }
            }

            for (var kb = 0; kb < BOARD_POW; kb++)
            {
                for (var kw = 0; kw < BOARD_POW; kw++)
                {
                    for (var pos = 0; pos < BOARD_POW; pos++)
                    {
                        for (var eb = 0; eb < EFFECT_TRESHOLD + 1; eb++)
                        {
                            for (var ew = 0; ew < EFFECT_TRESHOLD + 1; ew++)
                            {
                                effectTable_[kb, kw, pos, eb, ew] =
                                    ourEffectTable_[kb, pos, eb] // 先手から見ると先手玉周りに先手の利きがあるのは+
                                    - theirEffectTable_[kb, pos, ew] // 先手から見ると先手玉周りに後手の利きがあるのは-
                                    - ourEffectTable_[kw, pos, ew] // 先手から見ると後手玉周りに後手の利きがあるのは-
                                    + theirEffectTable_[kw, pos, eb]; // 先手から見ると後手玉周りに先手の利きがあるのは+
                            }
                        }
                    }
                }
            }
        }

        public int Evaluate(Board board)
            => board.IsChackMate
                ? board.Teban ? B_CHECKMATE : W_CHECKMATE
                : PieceAndEffectValue(board);

        private int PieceAndEffectValue(Board board)
        {
            var res = 0;
            for (var y = 1; y <= BOARD_SIZE; y++)
            {
                for (var x = 1; x <= BOARD_SIZE; x++)
                {
                    var pos = new Position(x, y);

                    var (eb, ew) = board.Effects(pos);
                    eb = Math.Min(eb, EFFECT_TRESHOLD);
                    ew = Math.Min(ew, EFFECT_TRESHOLD);
                    var (kb, kw) = (new Position(Array.IndexOf(board.Pieces, KingB)), new Position(Array.IndexOf(board.Pieces, KingW)));
                    res += effectTable_[kb, kw, pos, eb, ew];

                    var piece = board.Pieces[pos];
                    if (piece.IsEmpty()) continue;
                    // 盤上の駒に対しては、その価値を1/10ほど減ずる。
                    res += pieceValues_[(int)piece] * 920 / 1024;
                }
            }
            return res + board.HandsBlack.Concat(board.HandsWhite).Where(x => x.IsPiece()).Sum(x => pieceValues_[(int)x])
                + random_.Next(21) - 10;
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
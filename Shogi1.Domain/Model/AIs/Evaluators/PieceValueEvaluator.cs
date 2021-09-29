using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Linq;
using static Shogi1.Domain.Consts;

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
}
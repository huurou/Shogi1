using Shogi1.Domain.Model.Pieces;
using System.Collections.Generic;
using static Shogi1.Domain.Consts;

namespace Shogi1.Domain.Model.Boards
{
    /// <summary>
    /// 盤
    /// </summary>
    public class Board
    {
        private readonly Piece?[,] board_;
        private readonly List<Piece> handsPlayer_ = new();
        private readonly List<Piece> handsOpponent_ = new();

        /// <summary>
        /// 手番　True:先手番/False:後手番
        /// </summary>
        public bool Teban { get; }

        public int Turns { get; private set; }

        /// <summary>
        /// 初期局面
        /// </summary>
        public Board()
        {
            Teban = true;
            Turns = 0;
            board_ = new Piece?[,]
            {
                { new Kyosha(false), new Keima(false),new Ginsho(false),new Kinsho(false), new Osho(false),new Kinsho(false),new Ginsho(false),  new Keima(false),new Kyosha(false), },
                {              null, new Hisha(false),             null,             null,            null,             null,             null,new Kakugyo(false),             null, },
                {  new Fuhyo(false), new Fuhyo(false), new Fuhyo(false), new Fuhyo(false),new Fuhyo(false), new Fuhyo(false), new Fuhyo(false),  new Fuhyo(false), new Fuhyo(false), },
                {              null,             null,             null,             null,            null,             null,             null,              null,             null, },
                {              null,             null,             null,             null,            null,             null,             null,              null,             null, },
                {              null,             null,             null,             null,            null,             null,             null,              null,             null, },
                {   new Fuhyo(true),  new Fuhyo(true),  new Fuhyo(true),  new Fuhyo(true), new Fuhyo(true),  new Fuhyo(true),  new Fuhyo(true),   new Fuhyo(true),  new Fuhyo(true), },
                {              null,new Kakugyo(true),             null,             null,            null,             null,             null,   new Hisha(true),             null, },
                {  new Kyosha(true),  new Keima(true), new Ginsho(true), new Kinsho(true),  new Osho(true), new Kinsho(true), new Ginsho(true),   new Keima(true), new Kyosha(true), },
            };
        }

        public List<Move> LegalMoves
        {
            get
            {
                if (legalMoves_ is not null) return legalMoves_;
                var lms = new List<Move> { Capacity = 128 };
                for (var y = 0; y < BOARD_SIZE; y++)
                {
                    for (var x = 0; x < BOARD_SIZE; x++)
                    {
                        if (board_[y, x] is not Piece piece || piece.Teban != Teban) continue;
                        var movement = piece.GetMovement(new(x, y));
                        switch (piece)
                        {
                            case Osho osho:

                                break;
                            default:
                                break;
                        }
                    }
                }
                legalMoves_ = lms;
                return legalMoves_;
            }
        }
        private List<Move>? legalMoves_;

        public void Move(Move move)
        {
        }
    }

    public class Position
    {
        /// <summary>
        /// X座標 水平方向左から0始まり
        /// </summary>
        public int X { get; }
        /// <summary>
        /// Y座標 垂直方向上から0始まり
        /// </summary>
        public int Y { get; }

        internal Position Upper => new(X, Y - 1);
        internal Position UpperRight => new(X + 1, Y - 1);
        internal Position Right => new(X + 1, Y);
        internal Position LowerRight => new(X + 1, Y + 1);
        internal Position Lower => new(X, Y + 1);
        internal Position LowerLeft => new(X - 1, Y + 1);
        internal Position Left => new(X - 1, Y);
        internal Position UpperLeft => new(X - 1, Y - 1);

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Move
    {
        public Position From { get; }
        public Position To { get; }
        public bool Promote { get; }

        public Move(Position from, Position to, bool promote)
        {
            From = from;
            To = to;
            Promote = promote;
        }
    }
}
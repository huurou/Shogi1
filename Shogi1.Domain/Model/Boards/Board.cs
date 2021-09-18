using Shogi1.Domain.Model.Pieces;
using System;
using System.Collections.Generic;

namespace Shogi1.Domain.Model.Boards
{
    public class Move
    {
        public Position? From { get; }
        public Position To { get; }
        public Piece Piece { get; }
        public bool Promoted { get; }

        // Move
        public Move(Position from, Position to, Piece piece, bool promoted = false)
        {
            From = from;
            To = to;
            Piece = piece;
            Promoted = promoted;
        }

        // Drop
        public Move(Position to, Piece piece)
        {
            To = to;
            Piece = piece;
        }
    }

    /// <summary>
    /// 盤
    /// </summary>
    public class Board
    {
        private readonly Piece?[] board_;
        private readonly Gyoku kingPlayer_;
        private readonly Gyoku kingOpponent_;
        private readonly List<Piece> handsPlayer_ = new();
        private readonly List<Piece> handsOpponent_ = new();

        /// <summary>
        /// 手番　True:先手番/False:後手番
        /// </summary>
        public bool Teban { get; private set; }

        public int Turns { get; private set; }

        /// <summary>
        /// 初期局面
        /// </summary>
        public Board()
        {
            Teban = true;
            Turns = 0;
            board_ = new Piece?[]
            {
                new Kyou(false),   new Kei(false), new Gin(false), new Kin(false), new Gyoku(false), new Kin(false), new Gin(false),  new Kei(false), new Kyou(false),
                           null, new Hisya(false),           null,           null,             null,           null,           null, new Kaku(false),            null,
                  new Hu(false),    new Hu(false),  new Hu(false),  new Hu(false),    new Hu(false),  new Hu(false),  new Hu(false),   new Hu(false),   new Hu(false),
                           null,             null,           null,           null,             null,           null,           null,            null,            null,
                           null,             null,           null,           null,             null,           null,           null,            null,            null,
                           null,             null,           null,           null,             null,           null,           null,            null,            null,
                   new Hu(true),     new Hu(true),   new Hu(true),   new Hu(true),     new Hu(true),   new Hu(true),   new Hu(true),    new Hu(true),    new Hu(true),
                           null,   new Kaku(true),           null,           null,             null,           null,           null, new Hisya(true),            null,
                 new Kyou(true),    new Kei(true),  new Gin(true),  new Kin(true),  new Gyoku(true),  new Kin(true),  new Gin(true),   new Kei(true),  new Kyou(true),
            };
            kingPlayer_ = (Gyoku)board_[new Position(5, 9)]!;
            kingOpponent_ = (Gyoku)board_[new Position(5, 1)]!;
        }

        internal void Move(Move move)
        {
            var to = move.To;
            var piece = move.Piece;
            var promoted = move.Promoted;
            var hands = (Teban ? handsPlayer_ : handsOpponent_);
            // Move
            if (move.From is Position from)
            {
                if (board_[to] is Piece captured)
                {
                    if (captured.Teban == Teban) throw new InvalidOperationException("味方の駒を取ることはできません。");
                    captured.Reverse();
                    if (captured is Promotable promotable && promotable.Promoted) promotable.Unpromote();
                    hands.Add(captured);
                }
                if (promoted)
                {
                    if (piece is not Promotable promotable1) throw new InvalidOperationException($"{piece}は成れません。");
                    if (promotable1.Promoted) throw new InvalidOperationException($"{promotable1}はすでに成っています。");
                    if (Teban ? (from.Y > 3 && to.Y > 3) : (from.Y is < 6 && to.Y < 6)) throw new InvalidOperationException($"成りが可能な場所からあるいは場所へ移動していません。");
                    promotable1.Promote();
                }
                if (board_[from] != piece) throw new InvalidOperationException("");
                board_[from] = null;
                board_[to] = piece;
                if (IsCheck(Teban)) throw new InvalidOperationException();
            }
            // Drop
            else
            {
                if (!hands.Contains(piece)) throw new InvalidOperationException();
                hands.Remove(piece);
                if (board_[to] is not null) throw new InvalidOperationException();
                board_[to] = piece;
            }
        }

        internal bool IsCheck(bool teban)
        {
            if (teban)
            {
                var king = new Position(Array.IndexOf(board_, kingPlayer_));

                // 上
                if (king.X > 1)
                {
                    var position = king.Up();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Teban != teban ||
                        piece is Hu hu && hu.Teban != teban) return true;
                    while (position.X > 1 && board_[position] is null) position = position.Up();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban ||
                        piece is Kyou kyou1 && !kyou1.Promoted && kyou1.Teban != teban) return true;
                }
                //右上
                if (king.X > 1 && king.Y > 1)
                {
                    var position = king.UpRight();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (position.X > 1 && king.Y > 1 && board_[position] is null) position = position.UpRight();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
                // 右
                if (king.Y > 1)
                {
                    var position = king.Right();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Promoted && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (king.Y > 1 && board_[position] is null) position = position.Right();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban) return true;
                }
                // 右下
                if (king.X < 9 && king.Y > 1)
                {
                    var position = king.DownRight();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Gin gin && !gin.Promoted && gin.Teban != teban) return true;
                    while (king.X < 9 && king.Y > 1 && board_[position] is null) position = position.DownRight();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
                // 下
                if (king.X < 9)
                {
                    var position = king.Down();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Promoted && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (king.X < 9 && board_[position] is null) position = position.Down();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban) return true;
                }
                // 左下
                if (king.X < 9 && king.Y < 9)
                {
                    var position = king.DownLeft();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Gin gin && !gin.Promoted && gin.Teban != teban) return true;
                    while (king.X < 9 && king.Y < 9 && board_[position] is null) position = position.DownLeft();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
                //左
                if (king.Y < 9)
                {
                    var position = king.Left();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Promoted && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (king.Y < 9 && board_[position] is null) position = position.Left();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban) return true;
                }
                // 左上
                if (king.X > 1 && king.Y < 9)
                {
                    var position = king.UpLeft();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (position.X > 1 && king.Y < 9 && board_[position] is null) position = position.UpLeft();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
            }
            else
            {
                var king = new Position(Array.IndexOf(board_, kingOpponent_));

                // 上
                if (king.X > 1)
                {
                    var position = king.Up();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Promoted && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (king.X > 1 && board_[position] is null) position = position.Up();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban) return true;
                }
                //右上
                if (king.X > 1 && king.Y > 1)
                {
                    var position = king.UpRight();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Gin gin && !gin.Promoted && gin.Teban != teban) return true;
                    while (king.X > 1 && king.Y > 1 && board_[position] is null) position = position.UpRight();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
                // 右
                if (king.Y > 1)
                {
                    var position = king.Right();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Promoted && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (king.Y > 1 && board_[position] is null) position = position.Right();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban) return true;
                }
                // 右下
                if (king.X < 9 && king.Y > 1)
                {
                    var position = king.DownRight();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (position.X < 9 && king.Y > 1 && board_[position] is null) position = position.DownRight();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
                // 下
                if (king.X < 9)
                {
                    var position = king.Down();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Teban != teban ||
                        piece is Hu hu && hu.Teban != teban) return true;
                    while (position.X < 9 && board_[position] is null) position = position.Down();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban ||
                        piece is Kyou kyou1 && !kyou1.Promoted && kyou1.Teban != teban) return true;
                }
                // 左下
                if (king.X < 9 && king.Y < 9)
                {
                    var position = king.DownLeft();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (position.X < 9 && king.Y < 9 && board_[position] is null) position = position.DownLeft();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
                //左
                if (king.Y < 9)
                {
                    var position = king.Left();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Promoted && kaku.Teban != teban ||
                        piece is Kin kin && kin.Teban != teban ||
                        piece is Gin gin && gin.Promoted && gin.Teban != teban ||
                        piece is Kei kei && kei.Promoted && kei.Teban != teban ||
                        piece is Kyou kyou && kyou.Promoted && kyou.Teban != teban ||
                        piece is Hu hu && hu.Promoted && hu.Teban != teban) return true;
                    while (king.Y < 9 && board_[position] is null) position = position.Left();
                    piece = board_[position];
                    if (piece is Hisya hisya1 && hisya1.Teban != teban) return true;
                }
                // 左上
                if (king.X > 1 && king.Y < 9)
                {
                    var position = king.UpLeft();
                    var piece = board_[position];
                    if (piece is Gyoku gyoku && gyoku.Teban != teban ||
                        piece is Hisya hisya && hisya.Promoted && hisya.Teban != teban ||
                        piece is Kaku kaku && kaku.Teban != teban ||
                        piece is Gin gin && !gin.Promoted && gin.Teban != teban) return true;
                    while (king.X > 1 && king.Y < 9 && board_[position] is null) position = position.UpLeft();
                    piece = board_[position];
                    if (piece is Kaku kaku1 && kaku1.Teban != teban) return true;
                }
            }
            return false;
        }
    }

    public class Position
    {
        public int Value { get; }
        /// <summary>
        /// 水平方向 右から左 9 8 7 6 5 4 3 2 1
        /// </summary>
        public int X { get; }
        /// <summary>
        /// 垂直方向 上から下 一 二 三 四 五 六 七 八 九
        /// </summary>
        public int Y { get; }

        public Position(int value)
        {
            if (value is >= 0 and < 81)
            {
                Value = value;
                X = 9 - value % 9;
                Y = value / 9 + 1;
            }
            else throw new InvalidOperationException();
        }

        public Position(int x, int y)
        {
            if (x is >= 0 and < 9 && y is >= 0 and < 9)
            {
                Value = y * 9 - x;
                X = x;
                Y = y;
            }
            else throw new InvalidOperationException();
        }

        public Position Up() => new(X, Y - 1);

        public Position UpRight() => new(X - 1, Y - 1);

        public Position Right() => new(X - 1, Y);

        public Position DownRight() => new(X - 1, Y + 1);

        public Position Down() => new(X, Y + 1);

        public Position DownLeft() => new(X + 1, Y + 1);

        public Position Left() => new(X + 1, Y);

        public Position UpLeft() => new(X + 1, Y - 1);

        public Position JumpUpLeft() => new(X + 1, Y - 2);

        public Position JumpUpRight() => new(X - 1, Y - 2);

        public Position JumpDownLeft() => new(X + 1, Y + 2);

        public Position JumpDownRight() => new(X - 1, Y + 2);

        public static implicit operator int(Position position) => position.Value;
    }
}
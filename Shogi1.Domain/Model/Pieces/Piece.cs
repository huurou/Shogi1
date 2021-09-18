using Shogi1.Domain.Model.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;

namespace Shogi1.Domain.Model.Pieces
{
    public abstract class Piece
    {
        public abstract bool Teban { get; set; }

        /// <summary>
        /// 盤上にこの駒以外何もない時に移動可能なPositionを返す <para/>
        /// 内側のリスト 一方向についての着手可能箇所 途中に着手可能でない手が現れた場合それ以降も着手できない <br/>
        /// 外側のリスト 各方向についての着手可能リスト 途中で着手可能でない手が現れた場合次のリストに移る
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public abstract List<List<Position>> GetMovement(Position from);
    }

    public abstract class Promotable : Piece
    {
        public bool Promoted { get; private set; }

        internal void Promote()
        {
            if (Promoted) return;
            Promoted = true;
        }
    }

    public abstract class Unpromotable : Piece { }

    /// <summary>
    /// 王将 / 玉将
    /// </summary>
    public class Osho : Unpromotable
    {
        public override bool Teban { get; set; }

        public Osho(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var res = new List<List<Position>>() { Capacity = 8 };
            foreach (var p in new Position[]
            {
                from.Upper,
                from.UpperRight,
                from.Right,
                from.LowerRight,
                from.Lower,
                from.LowerLeft,
                from.Left,
                from.UpperLeft,
            }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            return res;
        }
    }

    /// <summary>
    /// 飛車 / 龍王
    /// </summary>
    public class Hisha : Promotable
    {
        public override bool Teban { get; set; }

        public Hisha(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var x = from.X;
            var y = from.Y;
            var e = BOARD_SIZE - 1;
            var res = new List<List<Position>>() { Capacity = 8 };
            res.Add(Enumerable.Range(1, y).Select(i => new Position(x, y - i)).ToList()); // ↑
            res.Add(Enumerable.Range(x + 1, e - x).Select(i => new Position(i, y)).ToList()); // →
            res.Add(Enumerable.Range(y + 1, e - y).Select(i => new Position(x, i)).ToList()); // ↓
            res.Add(Enumerable.Range(1, x).Select(i => new Position(x - i, y)).ToList()); // ←
            if (Promoted)
            {
                foreach (var p in new Position[]
               {
                from.UpperRight,
                from.LowerRight,
                from.LowerLeft,
                from.UpperLeft,
               }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            return res;
        }
    }

    /// <summary>
    /// 角行 / 龍馬
    /// </summary>
    public class Kakugyo : Promotable
    {
        public override bool Teban { get; set; }

        public Kakugyo(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var x = from.X;
            var y = from.Y;
            var e = BOARD_SIZE - 1;
            var res = new List<List<Position>>() { Capacity = 8 };
            res.Add(Enumerable.Range(1, Math.Min(e - x, y)).Select(i => new Position(x + i, y - i)).ToList()); // ↗
            res.Add(Enumerable.Range(1, Math.Min(e - x, e - y)).Select(i => new Position(x + i, y + i)).ToList()); // ↘
            res.Add(Enumerable.Range(1, Math.Min(x, e - y)).Select(i => new Position(x - i, y + i)).ToList()); // ↙
            res.Add(Enumerable.Range(1, Math.Min(x, y)).Select(i => new Position(x - i, y - i)).ToList()); // ↖
            if (Promoted)
            {
                foreach (var p in new Position[]
               {
                from.Upper,
                from.Right,
                from.Lower,
                from.Left,
               }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            return res;
        }
    }

    /// <summary>
    /// 金将
    /// </summary>
    public class Kinsho : Unpromotable
    {
        public override bool Teban { get; set; }

        public Kinsho(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var res = new List<List<Position>>() { Capacity = 8 };
            foreach (var p in new Position[]
            {
                    from.Upper,
                    from.UpperRight,
                    from.Right,
                    from.Lower,
                    from.Left,
                    from.UpperLeft,
            }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            return res;
        }
    }

    /// <summary>
    /// 銀将 / 成銀
    /// </summary>
    public class Ginsho : Promotable
    {
        public override bool Teban { get; set; }

        public Ginsho(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var res = new List<List<Position>>() { Capacity = 8 };
            if (Promoted)
            {
                foreach (var p in new Position[]
                {
                    from.Upper,
                    from.UpperRight,
                    from.Right,
                    from.Lower,
                    from.Left,
                    from.UpperLeft,
                }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            else
            {
                foreach (var p in new Position[]
                {
                    from.Upper,
                    from.UpperRight,
                    from.LowerRight,
                    from.LowerLeft,
                    from.UpperLeft,
                }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            return res;
        }
    }

    /// <summary>
    /// 桂馬 / 成桂
    /// </summary>
    public class Keima : Promotable
    {
        public override bool Teban { get; set; }

        public Keima(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var x = from.X;
            var y = from.Y;
            var e = BOARD_SIZE - 1;
            var res = new List<List<Position>>() { Capacity = 8 };
            if (Promoted)
            {
                foreach (var p in new Position[]
                {
                    from.Upper,
                    from.UpperRight,
                    from.Right,
                    from.Lower,
                    from.Left,
                    from.UpperLeft,
                }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            else
            {
                if (x != 0 && y >= 2) res.Add(new() { new(x - 1, y - 2) });
                if (x != e && y >= 2) res.Add(new() { new(x + 1, y - 2) });
            }
            return res;
        }
    }

    /// <summary>
    /// 香車 / 成香
    /// </summary>
    public class Kyosha : Promotable
    {
        public override bool Teban { get; set; }

        public Kyosha(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var x = from.X;
            var y = from.Y;
            var e = BOARD_SIZE - 1;
            var res = new List<List<Position>>() { Capacity = 8 };
            if (Promoted)
            {
                foreach (var p in new Position[]
                {
                    from.Upper,
                    from.UpperRight,
                    from.Right,
                    from.Lower,
                    from.Left,
                    from.UpperLeft,
                }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            else res.Add(Enumerable.Range(1, y).Select(i => new Position(x, y - i)).ToList());
            return res;
        }
    }

    /// <summary>
    /// 歩兵 / と金
    /// </summary>
    public class Fuhyo : Promotable
    {
        public override bool Teban { get; set; }

        public Fuhyo(bool teban) => Teban = teban;

        public override List<List<Position>> GetMovement(Position from)
        {
            var x = from.X;
            var y = from.Y;
            var e = BOARD_SIZE - 1;
            var res = new List<List<Position>>() { Capacity = 8 };
            if (Promoted)
            {
                foreach (var p in new Position[]
                {
                    from.Upper,
                    from.UpperRight,
                    from.Right,
                    from.Lower,
                    from.Left,
                    from.UpperLeft,
                }) if (p.X is >= 0 and < BOARD_SIZE && p.Y is >= 0 and < BOARD_SIZE) res.Add(new() { p });
            }
            else
            {
                if (y != 0) res.Add(new() { from.Upper });
            }
            return res;
        }
    }
}
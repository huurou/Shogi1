using System;
using System.Collections.Generic;
using System.Linq;

namespace Shogi1.Domain.Model.Boards
{
    /// <summary>
    /// 位置
    /// </summary>
    internal class Position
    {
        /// <summary>
        /// 左上を0とする盤上のインデックス
        /// </summary>
        internal int Value { get; }
        /// <summary>
        /// 水平方向 右向き 1始まり
        /// </summary>
        internal int X { get; }
        /// <summary>
        /// 垂直方向 下向き 1始まり
        /// </summary>
        internal int Y { get; }

        /// <summary>
        /// 盤上を指しているかどうか
        /// </summary>
        internal bool IsOnBoard => X is >= 1 and <= 9 && Y is >= 1 and <= 9;

        internal Position(int value)
        {
            Value = value;
            X = 9 - value % 9;
            Y = value / 9 + 1;
        }

        internal Position(int x, int y)
        {
            Value = y * 9 - x;
            X = x;
            Y = y;
        }

        internal Position Up() => new(X, Y - 1);

        internal Position UpRight() => new(X - 1, Y - 1);

        internal Position Right() => new(X - 1, Y);

        internal Position DownRight() => new(X - 1, Y + 1);

        internal Position Down() => new(X, Y + 1);

        internal Position DownLeft() => new(X + 1, Y + 1);

        internal Position Left() => new(X + 1, Y);

        internal Position UpLeft() => new(X + 1, Y - 1);

        internal Position JumpUpLeft() => new(X + 1, Y - 2);

        internal Position JumpUpRight() => new(X - 1, Y - 2);

        internal Position JumpDownLeft() => new(X + 1, Y + 2);

        internal Position JumpDownRight() => new(X - 1, Y + 2);

        /// <summary>
        /// fromからtoまで一直線に並んでいる時、間にあるPosition
        /// fromとto自身は含まない
        /// </summary>
        /// <param name="from">出発元</param>
        /// <param name="to">行き先</param>
        /// <returns>間にあるPosition</returns>
        internal static IEnumerable<Position> Range(Position from, Position to)
        {
            var (fx, fy, tx, ty) = (from.X, from.Y, to.X, to.Y);
            if (fx == tx)
            {
                // 下
                return fy < ty ? Enumerable.Range(fy + 1, ty - fy - 1).Select(i => new Position(fx, i))
                    // 上
                    : fy > ty ? Enumerable.Range(ty + 1, fy - ty - 1).Reverse().Select(i => new Position(fx, i))
                    : throw new InvalidOperationException();
            }
            if (fy == ty)
            {
                // 右
                return fx < tx ? Enumerable.Range(fx + 1, tx - fx - 1).Select(i => new Position(i, fy))
                    // 左
                    : fx > tx ? Enumerable.Range(tx + 1, fx - tx - 1).Reverse().Select(i => new Position(i, fy))
                    : throw new InvalidOperationException();
            }
            if (fx + fy == tx + ty)
            {
                // 左上
                return fx < tx ? Enumerable.Range(1, tx - fx - 1).Select(i => new Position(fx + i, fy - i))
                    // 右下
                    : fx > tx ? Enumerable.Range(1, fx - tx - 1).Select(i => new Position(fx - i, fy + i))
                    : throw new InvalidOperationException();
            }
            if (fx - fy == tx - ty)
            {
                // 左下
                return fx < tx ? Enumerable.Range(1, tx - fx - 1).Select(i => new Position(fx + i, fy + i))
                    // 右上
                    : fx > tx ? Enumerable.Range(1, fx - tx - 1).Select(i => new Position(fx - i, fy - i))
                    : throw new InvalidOperationException();
            }
            throw new InvalidOperationException();
        }

        public static implicit operator int(Position position) => position.Value;

        public override string ToString()
        {
            var x = X switch
            {
                0 => "０",
                1 => "１",
                2 => "２",
                3 => "３",
                4 => "４",
                5 => "５",
                6 => "６",
                7 => "７",
                8 => "８",
                9 => "９",
                10 => "１０",
                _ => "　",
            };
            var y = Y switch
            {
                0 => "〇",
                1 => "一",
                2 => "二",
                3 => "三",
                4 => "四",
                5 => "五",
                6 => "六",
                7 => "七",
                8 => "八",
                9 => "九",
                10 => "十",
                _ => "　",
            };
            return $"{x}{y}";
        }
    }
}
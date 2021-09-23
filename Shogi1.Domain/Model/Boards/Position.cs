using System;
using System.Collections.Generic;
using System.Linq;

namespace Shogi1.Domain.Model.Boards
{
    public class Position
    {
        public int Value { get; }
        public int X { get; }
        public int Y { get; }

        public bool OnBoard => X is >= 1 and <= 9 && Y is >= 1 and <= 9;

        public Position(int value)
        {
            Value = value;
            X = 10 - value % 11;
            Y = value / 11;
        }

        public Position(int x, int y)
        {
            Value = 10 - x + y * 11;
            X = x;
            Y = y;
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

        public static IEnumerable<Position> Range(Position from, Position to)
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
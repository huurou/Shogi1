namespace Shogi1.Application.Plays
{
    /// <summary>
    /// 位置
    /// </summary>
    public class Position
    {
        /// <summary>
        /// 左上を0とする盤上のインデックス
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// 水平方向 右向き 1始まり
        /// </summary>
        public int X { get; }

        /// <summary>
        /// 垂直方向 下向き 1始まり
        /// </summary>
        public int Y { get; }

        public Position(int value)
        {
            Value = value;
            X = 9 - value % 9;
            Y = value / 9 + 1;
        }

        public Position(int x, int y)
        {
            Value = y * 9 - x;
            X = x;
            Y = y;
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
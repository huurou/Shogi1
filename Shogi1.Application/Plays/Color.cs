using static Shogi1.Application.Plays.Color;

namespace Shogi1.Application.Plays
{
    /// <summary>
    /// 手番
    /// </summary>
    public enum Color
    {
        /// <summary>
        /// 先手
        /// </summary>
        Black,

        /// <summary>
        /// 後手
        /// </summary>
        White,
    }

    public static class ColorExtension
    {
        /// <summary>
        /// 先手かどうか
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool IsBlack(this Color color) => color == Black;

        /// <summary>
        /// 後手かどうか
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool IsWhite(this Color color) => color == White;

        /// <summary>
        /// 手番を入れ替える
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Switch(this Color color) => color.IsBlack() ? White : Black;
    }
}
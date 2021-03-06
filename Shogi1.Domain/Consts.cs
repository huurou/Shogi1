namespace Shogi1.Domain
{
    internal static class Consts
    {
        /// <summary>
        /// 先手
        /// </summary>
        internal const bool BLACK = true;

        /// <summary>
        /// 後手
        /// </summary>
        internal const bool WHITE = false;

        internal const int BOARD_SIZE = 9;

        internal const int BOARD_POW = BOARD_SIZE * BOARD_SIZE;

        /// <summary>
        /// 先手玉が詰んでいるときの評価値
        /// </summary>
        internal const int B_CHECKMATE = -99999999;

        /// <summary>
        /// 後手玉が詰んでいるときの評価値
        /// </summary>
        internal const int W_CHECKMATE = 99999999;
    }
}
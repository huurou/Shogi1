namespace Shogi1.Domain
{
    public static class Consts
    {
        /// <summary>
        /// 先手
        /// </summary>
        public const bool BLACK = true;

        /// <summary>
        /// 後手
        /// </summary>
        public const bool WHITE = false;

        /// <summary>
        /// 先手玉が詰んでいるときの評価値
        /// </summary>
        public const int B_CHECKMATE = -99999999;

        /// <summary>
        /// 後手玉が詰んでいるときの評価値
        /// </summary>
        public const int W_CHECKMATE = 99999999;
    }
}
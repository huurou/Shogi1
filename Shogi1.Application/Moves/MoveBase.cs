using Shogi1.Application.Plays;

namespace Shogi1.Application.Moves
{
    /// <summary>
    /// 駒の移動及び駒打ち
    /// </summary>
    public abstract class MoveBase
    {
        /// <summary>
        /// この手を指した手番
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// 移動先/打った位置
        /// </summary>
        public Position To { get; }

        /// <summary>
        /// 指された駒
        /// </summary>
        public Piece Piece { get; }

        /// <summary>
        /// 評価値
        /// </summary>
        public int Eval { get; }

        protected MoveBase(Color color, Position to, Piece piece, int eval)
        {
            Color = color;
            To = to;
            Piece = piece;
            Eval = eval;
        }
    }
}
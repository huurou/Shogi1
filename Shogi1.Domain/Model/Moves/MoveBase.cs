using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;

namespace Shogi1.Domain.Model.Moves
{
    /// <summary>
    /// 指し手
    /// </summary>
    public abstract class MoveBase
    {
        /// <summary>
        /// 指し手の手番
        /// </summary>
        internal bool Teban { get; private set; }
        /// <summary>
        /// 指された駒
        /// </summary>
        public Piece Piece { get; private set; }
        /// <summary>
        /// 駒を移動/打った場所
        /// </summary>
        public Position To { get; private set; }

        protected internal MoveBase(bool teban, Piece piece, Position to)
        {
            Teban = teban;
            Piece = piece;
            To = to;
        }
    }
}
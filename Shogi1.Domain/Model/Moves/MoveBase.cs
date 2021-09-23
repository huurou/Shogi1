using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;

namespace Shogi1.Domain.Model.Moves
{
    public abstract class MoveBase
    {
        public bool Teban { get; }
        public Piece Piece { get; }
        public Position To { get; }

        protected MoveBase(bool teban, Piece piece, Position to)
        {
            Teban = teban;
            Piece = piece;
            To = to;
        }
    }
}
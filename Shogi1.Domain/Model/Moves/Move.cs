using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;
using System.Text;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.Moves
{
    /// <summary>
    /// 駒を動かす手
    /// </summary>
    public class Move : MoveBase
    {
        public Position From { get; }
        internal bool Captured { get; }
        internal Piece PieceCaptured { get; }
        public bool Promoted { get; }

        internal Move(bool teban, Piece piece, Position to, Position from,
                    bool captured = false, Piece pieceCaptured = 空, bool promoted = false)
            : base(teban, piece, to)
        {
            From = from;
            Captured = captured;
            PieceCaptured = pieceCaptured;
            Promoted = promoted;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Teban ? "先手" : "後手");
            sb.Append(' ');
            sb.Append(From.ToString());
            sb.Append(' ');
            sb.Append(To.ToString());
            sb.Append(' ');
            sb.Append(Piece);
            sb.Append(Promoted ? "成" : "");
            if (Captured)
            {
                sb.Append(" captured:");
                sb.Append(PieceCaptured);
            }

            return sb.ToString();
        }
    }
}
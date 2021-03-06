using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;
using System.Text;

namespace Shogi1.Domain.Model.Moves
{
    /// <summary>
    /// 持ち駒を打つ手
    /// </summary>
    public class Drop : MoveBase
    {
        internal Drop(bool teban, Piece piece, Position to)
            : base(teban, piece, to) { }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Teban ? "先手" : "後手");
            sb.Append(' ');
            sb.Append(To.ToString());
            sb.Append(' ');
            sb.Append(Piece.ToLetters());
            sb.Append('打');

            return sb.ToString();
        }
    }
}
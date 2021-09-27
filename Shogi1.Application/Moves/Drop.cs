using Shogi1.Application.Plays;

namespace Shogi1.Application.Moves
{
    /// <summary>
    /// 駒打ち
    /// </summary>
    public class Drop : MoveBase
    {
        public Drop(Color color, Position to, Piece piece, int eval)
            : base(color, to, piece, eval) { }
    }
}
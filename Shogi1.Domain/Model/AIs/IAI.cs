using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;

namespace Shogi1.Domain.Model.AIs
{
    public interface IAI
    {
        /// <summary>
        /// 指し手を決定する
        /// </summary>
        /// <param name="board">盤</param>
        /// <returns>(指し手、評価値)</returns>
        (MoveBase moveBase, int eval) DecideMove(Board board);
    }
}
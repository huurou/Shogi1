using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;

namespace Shogi1.Domain.Model.AIs.Searchers
{
    internal interface ISearcher
    {
        /// <summary>
        /// 手を探索する
        /// </summary>
        /// <param name="board">盤</param>
        /// <param name="depth">深さ</param>
        /// <returns>(指し手、評価値)</returns>
        (MoveBase moveBase, int eval) Search(Board board, int depth);
    }
}
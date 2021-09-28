using Shogi1.Domain.Model.AIs.Searchers;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;

namespace Shogi1.Domain.Model.AIs
{
    /// <summary>
    /// 全てランダム着手
    /// </summary>
    internal class AllRandomAI : IAI
    {
        private readonly RandomSearcher searcher_ = new();

        public (MoveBase moveBase, int eval) DecideMove(Board board)
            => searcher_.Search(board, 0);
    }
}
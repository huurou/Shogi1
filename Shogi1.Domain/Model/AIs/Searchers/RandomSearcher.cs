using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;

namespace Shogi1.Domain.Model.AIs.Searchers
{
    /// <summary>
    /// ランダム着手
    /// </summary>
    internal class RandomSearcher : ISearcher
    {
        public (MoveBase moveBase, double eval) Search(Board board, int depth)
        {
            var lms = board.GetLegalMoves();
            return (lms[RandomProvider.Next(lms.Count)], 0);
        }
    }
}
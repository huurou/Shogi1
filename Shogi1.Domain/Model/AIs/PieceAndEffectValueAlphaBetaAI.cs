using Shogi1.Domain.Model.AIs.Evaluators;
using Shogi1.Domain.Model.AIs.Searchers;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;

namespace Shogi1.Domain.Model.AIs
{
    internal class PieceAndEffectValueAlphaBetaAI : IAI
    {
        private readonly ISearcher searcher_ = new AlphaBetaSearcher(new PieceValueAndEffectEvaluator());
        private readonly int depth_;

        internal PieceAndEffectValueAlphaBetaAI(int depth) => depth_ = depth;

        public (MoveBase moveBase, int eval) DecideMove(Board board) => searcher_.Search(board, depth_);
    }
}
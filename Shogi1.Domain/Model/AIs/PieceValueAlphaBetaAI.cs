using Shogi1.Domain.Model.AIs.Evaluators;
using Shogi1.Domain.Model.AIs.Searchers;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;

namespace Shogi1.Domain.Model.AIs
{
    public class PieceValueAlphaBetaAI : IAI
    {
        private readonly ISearcher searcher_ = new AlphaBetaSearcher(new PieceValueEvaluator());

        private readonly int depth_;

        public PieceValueAlphaBetaAI(int depth) => depth_ = depth;

        public (MoveBase moveBase, double eval) DecideMove(Board board) => searcher_.Search(board, depth_);
    }
}
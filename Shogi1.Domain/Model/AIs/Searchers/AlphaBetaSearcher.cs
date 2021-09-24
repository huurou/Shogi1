using Shogi1.Domain.Model.AIs.Evaluators;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shogi1.Domain.Model.AIs.Searchers
{
    /// <summary>
    /// αβ探索
    /// </summary>
    internal class AlphaBetaSearcher : ISearcher
    {
        private readonly IEvaluator evaluator_;

        public AlphaBetaSearcher(IEvaluator evaluator) => evaluator_ = evaluator;

        public (MoveBase moveBase, double eval) Search(Board board, int depth)
        {
            var lms = board.GetLegalMoves();
            var count = lms.Count;
            if (count == 0) throw new InvalidOperationException();
            if (count == 1)
            {
                board.DoMove(lms[0]);
                var ev = evaluator_.Evaluate(board);
                board.UndoMove();
                return (lms[0], ev);
            }
            var results = new double[count];
            var boards = Enumerable.Range(0, count).Select(_ => board.Clone());
            Parallel.For(0, count, i =>
            {
                var b = boards.ElementAt(i);
                b.DoMove(lms[i]);
                results[i] = AlphaBetaPruning(b, evaluator_, depth - 1);
                b.UndoMove();
            });
            var eval = board.Teban ? results.Max() : results.Min();
            var indices = results.Select((x, i) => (x, i)).Where(x => x.x == eval).Select(x => x.i);
            var index = indices.ElementAt(RandomProvider.Next(indices.Count()));
            return (lms[index], eval);
        }

        internal static double AlphaBetaPruning(Board board, IEvaluator evaluator, int depth,
                                                double alpha = double.NegativeInfinity,
                                                double beta = double.PositiveInfinity)
        {
            if (depth <= 0) return evaluator.Evaluate(board);
            var lms = board.GetLegalMoves();
            if (lms.Count == 0) return evaluator.Evaluate(board);
            if (board.Teban)
            {
                foreach (var lm in lms)
                {
                    board.DoMove(lm);
                    alpha = Math.Max(alpha, AlphaBetaPruning(board, evaluator, depth - 1, alpha, beta));
                    board.UndoMove();
                    if (alpha >= beta) break; // βカット
                }
                return alpha;
            }
            else
            {
                foreach (var lm in lms)
                {
                    board.DoMove(lm);
                    beta = Math.Min(beta, AlphaBetaPruning(board, evaluator, depth - 1, alpha, beta));
                    board.UndoMove();
                    if (alpha >= beta) break; // αカット
                }
                return beta;
            }
        }
    }
}
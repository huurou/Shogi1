using Shogi1.Domain.Model.AIs.Evaluators;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;
using System;

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
#if true
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            var eval = 0D;
            var index = 0;
            if (board.Teban)
            {
                eval = alpha;
                for (var i = 0; i < count; i++)
                {
                    board.DoMove(lms[i]);
                    alpha = Math.Max(alpha, AlphaBetaPruning(board, evaluator_, depth - 1, alpha, beta));
                    Console.WriteLine($"{lms[i],-30}{alpha}");
                    if (eval < alpha)
                    {
                        index = i;
                        eval = alpha;
                    }
                    board.UndoMove();
                }
            }
            else
            {
                eval = beta;
                for (var i = 0; i < count; i++)
                {
                    board.DoMove(lms[i]);
                    beta = Math.Min(beta, AlphaBetaPruning(board, evaluator_, depth - 1, alpha, beta));
                    Console.WriteLine($"{lms[i],-30}{beta}");
                    if (eval > beta)
                    {
                        index = i;
                        eval = beta;
                    }
                    board.UndoMove();
                }
            }
#else
            var results = new double[count];
            var boards = Enumerable.Range(0, count).Select(_ => board.Clone());
            Parallel.For(0, count, i =>
            {
                var b = boards.ElementAt(i);
                b.DoMove(lms[i]);
                results[i] = AlphaBetaPruning(b, evaluator_, depth - 1);
                Console.WriteLine($"{lms[i],-30}{results[i]}");
                b.UndoMove();
            });
            var eval = board.Teban ? results.Max() : results.Min();
            var indices = results.Select((x, i) => (x, i)).Where(x => x.x == eval).Select(x => x.i);
            var index = indices.ElementAt(RandomProvider.Next(indices.Count()));
#endif
            return (lms[index], eval);
        }

        internal static int AlphaBetaPruning(Board board, IEvaluator evaluator, int depth, int alpha, int beta)
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
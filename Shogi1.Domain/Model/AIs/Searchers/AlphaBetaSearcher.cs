using Shogi1.Domain.Model.AIs.Evaluators;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;
using Shogi1.Domain.Model.TTs;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shogi1.Domain.Consts;

namespace Shogi1.Domain.Model.AIs.Searchers
{
    internal class IDDFSearcher : ISearcher
    {
        private readonly IEvaluator evaluator_;

        public IDDFSearcher(IEvaluator evaluator) => evaluator_ = evaluator;

        public (MoveBase moveBase, int eval) Search(Board board, int depth)
        {
            var lms = TT.TryFetch(board.Hash, out var entry) ? entry.Moves : board.GetLegalMoves();
            var count = lms.Count;
            if (count == 0) throw new InvalidOperationException();
            if (count == 1)
            {
                board.DoMove(lms[0]);
                var ev = evaluator_.Evaluate(board);
                board.UndoMove();
                return (lms[0], ev);
            }
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            int eval;
            var index = 0;
            if (board.Teban)
            {
                eval = alpha;
                for (var i = 0; i < count; i++)
                {
                    board.DoMove(lms[i]);
                    alpha = Math.Max(alpha, IDAlphaBeta(board, evaluator_, depth - 1, alpha, beta));
                    if (eval < alpha)
                    {
                        index = i;
                        eval = alpha;
                        Console.WriteLine($"{lms[i],-40}{alpha}");
                    }
                    board.UndoMove();
                    if (eval == W_CHECKMATE) break;
                }
            }
            else
            {
                eval = beta;
                for (var i = 0; i < count; i++)
                {
                    board.DoMove(lms[i]);
                    beta = Math.Min(beta, IDAlphaBeta(board, evaluator_, depth - 1, alpha, beta));
                    if (eval > beta)
                    {
                        index = i;
                        eval = beta;
                        Console.WriteLine($"{lms[i],-40}{beta}");
                    }
                    board.UndoMove();
                    if (eval == B_CHECKMATE) break;
                }
            }
            return (lms[index], eval);
        }

        private static int IDAlphaBeta(Board board, IEvaluator evaluator, int depth, int alpha, int beta)
        {
            if (depth <= 0) return evaluator.Evaluate(board);
            var lms = TT.TryFetch(board.Hash, out var entry) ? entry.Moves : board.GetLegalMoves();
            if (lms.Count == 0) return evaluator.Evaluate(board);
            if (board.Teban)
            {
                var moves = new List<(MoveBase m, int e)>();
                foreach (var lm in lms)
                {
                    board.DoMove(lm);
                    alpha = Math.Max(alpha, IDAlphaBeta(board, evaluator, depth - 1, alpha, beta));
                    board.UndoMove();
                    moves.Add((lm, alpha));
                    if (alpha >= beta) break; // βカット
                }
                TT.Save(board.Hash, new(moves.OrderByDescending(x => x.e).Select(x => x.m).ToList()));
                return alpha;
            }
            else
            {
                var moves = new List<(MoveBase m, int e)>();
                foreach (var lm in lms)
                {
                    board.DoMove(lm);
                    beta = Math.Min(beta, IDAlphaBeta(board, evaluator, depth - 1, alpha, beta));
                    board.UndoMove();
                    moves.Add((lm, beta));
                    if (alpha >= beta) break; // αカット
                }
                TT.Save(board.Hash, new(moves.OrderBy(x => x.e).Select(x => x.m).ToList()));
                return beta;
            }
        }
    }

    /// <summary>
    /// αβ探索
    /// </summary>
    internal class AlphaBetaSearcher : ISearcher
    {
        private readonly IEvaluator evaluator_;

        internal AlphaBetaSearcher(IEvaluator evaluator) => evaluator_ = evaluator;

        public (MoveBase moveBase, int eval) Search(Board board, int depth)
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
            lms = Sort(board, evaluator_, lms);
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            int eval;
            var index = 0;
            if (board.Teban)
            {
                eval = alpha;
                for (var i = 0; i < count; i++)
                {
                    board.DoMove(lms[i]);
                    alpha = Math.Max(alpha, AlphaBetaPruning(board, evaluator_, depth - 1, alpha, beta));
                    if (eval < alpha)
                    {
                        index = i;
                        eval = alpha;
                        Console.WriteLine($"{lms[i],-40}{alpha}");
                    }
                    board.UndoMove();
                    if (eval == W_CHECKMATE) break;
                }
            }
            else
            {
                eval = beta;
                for (var i = 0; i < count; i++)
                {
                    board.DoMove(lms[i]);
                    beta = Math.Min(beta, AlphaBetaPruning(board, evaluator_, depth - 1, alpha, beta));
                    if (eval > beta)
                    {
                        index = i;
                        eval = beta;
                        Console.WriteLine($"{lms[i],-40}{beta}");
                    }
                    board.UndoMove();
                    if (eval == B_CHECKMATE) break;
                }
            }
            return (lms[index], eval);
        }

        private static int AlphaBetaPruning(Board board, IEvaluator evaluator, int depth, int alpha, int beta)
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

        private static List<MoveBase> Sort(Board board, IEvaluator evaluator, List<MoveBase> moves)
        {
            var mes = moves.Select(x =>
            {
                board.DoMove(x);
                var eval = AlphaBetaPruning(board, evaluator, 2, int.MinValue, int.MaxValue);
                board.UndoMove();
                return (x, eval);
            });
            return board.Teban
                ? mes.OrderByDescending(x => x.eval).Select(x => x.x).ToList()
                : mes.OrderBy(x => x.eval).Select(x => x.x).ToList();
        }
    }
}
using Shogi1.Domain.Model.Boards;

namespace Shogi1.Domain.Model.AIs.Evaluators
{
    internal interface IEvaluator
    {
        /// <summary>
        /// 盤面を評価して評価値を返す 先手有利:+ 後手有利:-
        /// </summary>
        /// <param name="board">盤</param>
        /// <returns>評価値</returns>
        int Evaluate(Board board);
    }
}
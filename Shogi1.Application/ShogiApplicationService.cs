using Shogi1.Domain.Model;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Games;
using Shogi1.Domain.Model.Moves;
using System;

namespace Shogi1.Application
{
    public class ShogiApplicationService
    {
        public event EventHandler<Board>? GameStart;
        public event EventHandler<(Board board, MoveBase move)>? Moved;
        public event EventHandler<Result>? GameEnd;

        private readonly Board board_ = new();

        public void Loop()
        {
            GameStart?.Invoke(this, board_);
            while (true)
            {
                var moves = board_.GetLegalMoves();
                if (moves.Count == 0) break;
                var move = moves[RandomProvider.Next(moves.Count)];
                board_.DoMove(move);
                Moved?.Invoke(this, (board_, move));
            }
            GameEnd?.Invoke(this, board_.Teban ? Result.Lose : Result.Win);
        }
    }
}
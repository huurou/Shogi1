using Shogi1.Domain.Model.AIs;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;
using System;

namespace Shogi1.Domain.Model.Games
{
    public class Game
    {
        internal event EventHandler<Board>? GameStart;
        internal event EventHandler<(Board board, MoveBase move, int eval)>? Moved;
        internal event EventHandler<Result>? GameEnd;

        private readonly IAI black_;
        private readonly IAI white_;
        private readonly Board board_ = new();

        private readonly IGameRepository gameRepository_;

        internal Game(IAI black, IAI white, IGameRepository gameRepository)
        {
            black_ = black;
            white_ = white;
            gameRepository_ = gameRepository;
        }

        internal void Run()
        {
            GameStart?.Invoke(this, board_);
            while (!board_.IsChackMate)
            {
                var (move, eval) = board_.Teban ? black_.DecideMove(board_) : white_.DecideMove(board_);
                board_.DoMove(move);
                Moved?.Invoke(this, (board_, move, eval));
            }
            var result = board_.Teban ? Result.Lose : Result.Win;
            GameEnd?.Invoke(this, result);
        }
    }
}
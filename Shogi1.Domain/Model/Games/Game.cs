using Shogi1.Domain.Model.AIs;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;
using System;

namespace Shogi1.Domain.Model.Games
{
    internal class Game
    {
        public event EventHandler<Board>? GameStart;
        public event EventHandler<(Board board, MoveBase move, double eval)>? Moved;
        public event EventHandler<Result>? GameEnd;

        private readonly IAI black_;
        private readonly IAI white_;
        private Board board_ = new();

        internal Game(IAI black, IAI white)
        {
            black_ = black;
            white_ = white;
        }

        internal void Run()
        {
            board_ = new();
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
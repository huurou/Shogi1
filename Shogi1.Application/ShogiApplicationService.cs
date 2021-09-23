using Shogi1.Domain.Model;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Moves;
using System;
using System.Collections.Generic;

namespace Shogi1.Application
{
    public class ShogiApplicationService
    {
        public event EventHandler<Board>? GameStart;
        public event EventHandler<(Board board, MoveBase move)>? Moved;

        private readonly Board board_ = new();

        public void Loop()
        {
            GameStart?.Invoke(this, board_);
            while (true)
            {
                Moved?.Invoke(this, RandomMove());
            }
        }

        public (Board board, MoveBase moveBase) RandomMove()
        {
            var moves = board_.GetPseudoMoves();
            var move = moves[RandomProvider.Next(moves.Count)];
            board_.DoMove(move);
            return (board_, move);
        }
    }
}
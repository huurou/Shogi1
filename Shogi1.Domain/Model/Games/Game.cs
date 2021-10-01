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

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public IAI Black { get; }
        public IAI White { get; }
        public Board Board { get; } = new();

        private readonly IGameRepository gameRepository_;

        internal Game(IAI black, IAI white, IGameRepository gameRepository)
        {
            Black = black;
            White = white;
            gameRepository_ = gameRepository;
        }

        internal void Run()
        {
            GameStart?.Invoke(this, Board);
            Start = DateTime.Now;
            gameRepository_.Save(this);
            while (!Board.IsChackMate)
            {
                var (move, eval) = Board.Teban ? Black.DecideMove(Board) : White.DecideMove(Board);
                Board.DoMove(move);
                Moved?.Invoke(this, (Board, move, eval));
                gameRepository_.Save(this);
            }
            var result = Board.Teban ? Result.Lose : Result.Win;
            GameEnd?.Invoke(this, result);
            End = DateTime.Now;
            gameRepository_.Save(this);
        }
    }
}
using Shogi1.Domain.Model.AIs;
using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Games;
using Shogi1.Domain.Model.Moves;
using System;

namespace Shogi1.Application
{
    public class ShogiApplicationService
    {
        public event EventHandler<Board>? GameStart;
        public event EventHandler<(Board board, MoveBase move, double eval)>? Moved;
        public event EventHandler<Result>? GameEnd;
        public event EventHandler<(int win, int lose, int draw)>? LoopEnd;

        private readonly Game game_;
        private int win_;
        private int lose_;
        private int draw_;

        public ShogiApplicationService(IAI black, IAI white)
        {
            game_ = new(black, white);

            game_.GameStart += (s, e) => GameStart?.Invoke(s, e);
            game_.Moved += (s, e) => Moved?.Invoke(s, e);
            game_.GameEnd += (s, e) =>
            {
                GameEnd?.Invoke(s, e);
                if (e == Result.Win) win_++;
                else if (e == Result.Lose) lose_++;
                else draw_++;
            };
        }

        public void GameLoop(int count)
        {
            for (var i = 0; i < count; i++)
            {
                game_.Run();
            }
            LoopEnd?.Invoke(this, (win_, lose_, draw_));
        }
    }
}
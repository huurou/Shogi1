using Shogi1.Application;
using Shogi1.Domain.Model.Boards;
using System;
using System.Text;

namespace Shogi1.Presentation.ConsolApp
{
    internal class Program
    {
        private static readonly ShogiApplicationService appService_ = new();

        private static void Main()
        {
            appService_.GameStart += (s, e) =>
            {
                Console.WriteLine(e);
                Console.WriteLine();
            };
            appService_.Moved += (s, e) =>
            {
                Console.WriteLine(e.move);
                Console.WriteLine(e.board);
                Console.WriteLine();
            };

            appService_.Loop();

        }

    }
}
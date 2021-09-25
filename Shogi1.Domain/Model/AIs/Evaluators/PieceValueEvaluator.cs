using Shogi1.Domain.Model.Boards;
using Shogi1.Domain.Model.Pieces;
using System;
using System.Linq;
using static Shogi1.Domain.Model.Pieces.Piece;

namespace Shogi1.Domain.Model.AIs.Evaluators
{
    internal class PieceValueEvaluator : IEvaluator
    {
        private readonly Random random_ = new();
        public double Evaluate(Board board)
        {
            return board.IsChackMate
                ? board.Teban ? -99999999 : 99999999
                : board.Pieces.Where(x=>x.IsPiece()).Sum(x => PieceValue(x)) 
                    + board.HandsBlack.Concat(board.HandsWhite).Where(x => x.IsPiece()).Sum(x => PieceValue(x)) * 1.113
                    + random_.NextDouble();

            static double PieceValue(Piece piece) => piece switch
            {
                王B => 0,
                飛B => 990,
                龍王B => 1395,
                角B => 855,
                龍馬B => 945,
                金B => 540,
                銀B => 495,
                成銀B => 540,
                桂B => 405,
                成桂B => 540,
                香B => 315,
                成香B => 540,
                歩B => 90,
                と金B => 540,
                王W => 0,
                飛W => -990,
                龍王W => -1395,
                角W => -855,
                龍馬W => -945,
                金W => -540,
                銀W => -495,
                成銀W => -540,
                桂W => -405,
                成桂W => -540,
                香W => -315,
                成香W => -540,
                歩W => -90,
                と金W => -540,
                空 => 0,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
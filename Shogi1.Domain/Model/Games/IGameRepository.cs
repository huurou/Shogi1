using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shogi1.Domain.Model.Games
{
    public interface IGameRepository
    {
        void Save(Game game);
    }
}

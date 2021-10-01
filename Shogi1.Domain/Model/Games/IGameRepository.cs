namespace Shogi1.Domain.Model.Games
{
    public interface IGameRepository
    {
        void Save(Game game);
    }
}
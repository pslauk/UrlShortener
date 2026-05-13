using UrlShortener.DomainModel.Interfaces;

namespace UrlShortener.DataAccess.Repositories
{
    public class UrlShortenerContextWorker : IContextWorker
    {
        private readonly UrlShortenerContext _dbContext;

        public UrlShortenerContextWorker(UrlShortenerContext context)
        {
            _dbContext = context;
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }
    }
}

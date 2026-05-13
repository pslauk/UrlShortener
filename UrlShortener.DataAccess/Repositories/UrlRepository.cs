using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UrlShortener.DomainModel.Entities;
using UrlShortener.DomainModel.Interfaces;

namespace UrlShortener.DataAccess.Repositories
{
    public class UrlRepository : IRepository<Url>
    {
        private readonly UrlShortenerContext _dbContext;

        public UrlRepository(UrlShortenerContext context)
        {
            _dbContext = context;
        }

        public IEnumerable<Url> GetAll(Expression<Func<Url, object>> include = null)
        {
            return _dbContext.Urls.ToList();
        }

        public Url GetById(int id)
        {
            return _dbContext.Urls.Find(id);
        }

        public IEnumerable<Url> Find(Expression<Func<Url, bool>> where, Expression<Func<Url, object>> include = null)
        {
            return _dbContext.Urls.Where(where).ToList();
        }

        public void Add(Url category)
        {
            _dbContext.Urls.Add(category);
        }

        public void Update(Url category)
        {
            _dbContext.Entry(category).State = EntityState.Modified;
        }

        public void Delete(Url category)
        {
            _dbContext.Urls.Attach(category);
            _dbContext.Urls.Remove(category);
        }
    }
}

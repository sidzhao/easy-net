using System.Collections.Generic;

namespace EasyNet.Data
{
    public class PagedEntities<TEntity>
    {
        public PagedEntities()
        {

        }

        public PagedEntities(int totalCount, List<TEntity> entities)
        {
            TotalCount = totalCount;
            Entities = entities;
        }

        public int TotalCount { get; set; }

        public List<TEntity> Entities { get; set; }
    }
}

﻿namespace Masa.Auth.Web.Admin.Rcl.Data.Base
{
    public class PagingData<TEntity> where TEntity : class
    {
        public int Page { get; private set; }

        public int PageSize { get; private set; }

        public long Count { get; private set; }

        public int PageCount => (int)Math.Ceiling(Count / (decimal)PageSize);

        public IEnumerable<TEntity> Items { get; private set; }

        public PagingData(int page, int pageSize, long count, IEnumerable<TEntity> items)
        {
            Page = page;
            PageSize = pageSize;
            Count = count;
            Items = items;
        }
    }
}
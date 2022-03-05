using System;
using System.Collections.Generic;

namespace MetricsAgent.DAL
{
	public interface IRepository<T> where T : class
    {
        IList<T> GetAll();

        IList<T> GetByTimePeriod(TimeSpan from, TimeSpan to);

        T GetById(int id);

        void Create(T item);

        void Update(T item);

        void Delete(int id);

    }
}


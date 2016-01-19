using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public interface BillRepository
    {
        IEnumerable<Bill> GetAllBills();
        void Delete(int id);
        void Save();
        void Add(Bill target);
    }
}

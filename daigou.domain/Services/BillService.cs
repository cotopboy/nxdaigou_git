using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain.Services
{
    public class BillService
    {
        private BillRepository repository;

        public BillService(BillRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<Bill> GetAllBills()
        {
            return this.repository.GetAllBills();
        }

        public void Delete(int targetId)
        {
            this.repository.Delete(targetId);
        }

        public void Save()
        {
            this.repository.Save();
        }

        public void Add(domain.Bill target)
        {
            this.repository.Add(target);
        }
    }
}

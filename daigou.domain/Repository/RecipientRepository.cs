using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public interface  RecipientRepository
    {
        IEnumerable<Recipient> GetAllRecipient();
        Recipient GetRecipient(string recipientName);
        Recipient GetRecipient(int id);
        void DeleteRecipient(int id);
        void SaveRecipient(domain.Recipient target);
        void AddRecipient(domain.Recipient target);
    }
}

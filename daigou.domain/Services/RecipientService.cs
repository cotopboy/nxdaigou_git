using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.domain
{
    public class RecipientService
    {
        private readonly RecipientRepository repository;

        public RecipientService(RecipientRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<Recipient> GetAllRecipient()
        {
            return this.repository.GetAllRecipient();
        }

        public Recipient GetRecipient(string recipientName)
        {
            return this.repository.GetRecipient(recipientName);
        }

        public Recipient GetRecipient(int recipient_id)
        {
            return this.repository.GetRecipient(recipient_id);
        }

        public void DeleteRecipient(int targetId)
        {
            this.repository.DeleteRecipient(targetId);
        }

        public void SaveRecipient(domain.Recipient target)
        {
            this.repository.SaveRecipient(target);
        }

        public void AddRecipient(domain.Recipient target)
        {
            this.repository.AddRecipient(target);
        }
    }
}

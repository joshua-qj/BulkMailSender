using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMailSender.Domain.Entities.Email
{
    public class JobSummary
    {
        public Guid? JobId { get; set; }
        public Guid EmailId { get; set; }
        public DateTime Date { get; set; }
        public string EmailFrom { get; set; }
        public string Subject { get; set; }
        public int TotalEmailsSent { get; set; }
        public int SuccessfulEmails { get; set; }
        public int FailedEmails { get; set; }
        public int PendingEmails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMailSender.Application.Dtos {
    public class EmailFailureRecord {
            public string Email { get; set; } = string.Empty; 
            public string ErrorMessage { get; set; } = string.Empty;
        }
}

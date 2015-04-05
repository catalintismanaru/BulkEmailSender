using System.Collections.Generic;

namespace Bulk_Email_Sender
{
    class InputData
    {
        public User User { get; set; }
        public string Subject { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }

        public List<string> EmailsFiles { get; set; }
        public List<string> AttachamentFiles { get; set; }
        public string TextMessage { get; set; }
    }
}

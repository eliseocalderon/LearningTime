using System;
using System.Diagnostics;

namespace LearningTime.Services
{
    public class DebugMailService : IMailService
    {
        public bool SendMail(string to, string from, string subject, string body)
        {
            Debug.WriteLine($"Send mail: To: {to}, Subject: {subject}");
            return true;
        }
    }
}

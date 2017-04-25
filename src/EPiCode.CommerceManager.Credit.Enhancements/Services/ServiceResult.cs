using System.Collections.Generic;

namespace CommerceManagerCreditEnhancements.Services
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}

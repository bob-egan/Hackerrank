using System.Collections.Generic;

namespace HackerRankChallenge
{
    public class ResponseObject
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public int totalPages { get; set; }
        public List<Transaction> data { get; set; }

    }
}

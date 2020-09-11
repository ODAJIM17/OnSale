using System;
using System.Collections.Generic;
using System.Text;

namespace OnSale.Common.Responses
{
    public class QualificationResponse
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateLocal => Date.ToLocalTime();

        public float Score { get; set; }

        public string Remarks { get; set; }
    }
}

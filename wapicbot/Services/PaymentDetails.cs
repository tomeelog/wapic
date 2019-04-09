using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wapicbot.Services
{
    [Serializable]
    public class PaymentDetails
    {
        public string CreditCardHolder { get; set; }

        public string CreditCardNumber { get; set; }
    }
}
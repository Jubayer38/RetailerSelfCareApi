using System.Data;

namespace Domain.StaticClass
{
    public class CustomerFeedback
    {
        public string customerName { get; set; }
        public string feedback { get; set; }
        public string rating { get; set; }

        public CustomerFeedback(DataRow dr)
        {
            this.customerName = dr["CUSTOMER_NAME"] as string;
            this.feedback = dr["FEEDBACK"] as string;
            this.rating = dr["RATING"] as string;
        }
    }
}

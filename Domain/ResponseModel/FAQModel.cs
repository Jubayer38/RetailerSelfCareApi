using System.Data;

namespace Domain.ResponseModel
{
    public class FAQModel
    {
        public string categoryTitle { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public FAQModel(DataRow dr)
        {
            categoryTitle = dr["CATEGORY_NAME"] as string;
            question = dr["QUESTION"] as string;
            answer = dr["ANSWER"] as string;
        }
    }
}

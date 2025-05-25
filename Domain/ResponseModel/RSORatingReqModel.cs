using Domain.RequestModel;

namespace Domain.ResponseModel
{
    public class RSORatingReqModel : RetailerRequestV2
    {
        public string name { get; set; }
        public string code { get; set; }
        public string number { get; set; }
        public double rating { get; set; }
        public string comment { get; set; }
        public string status { get; set; }

    }
}

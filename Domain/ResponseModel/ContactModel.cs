using System.Data;

namespace Domain.ResponseModel
{
    public class ContactModel
    {
        public long contactId { get; set; }
        public string contactName { get; set; }
        public string contactNo { get; set; }
        public string retailerCode { get; set; }

        public ContactModel(DataRow dr)
        {
            if (DBNull.Value != dr["CONTACT_ID"]) contactId = Convert.ToInt64(dr["CONTACT_ID"]);
            contactName = dr["CONTACT_NAME"] as string;
            contactNo = dr["CONTACT_NO"] as string;
            retailerCode = dr["RETAILER_CODE"] as string;
        }
    }
}
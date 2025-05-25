using System.Data;

namespace Domain.ViewModel
{
    public class ProductCategoryModel
    {
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public ProductCategoryModel(DataRow dr)
        {
            this.categoryId = dr["categoryId"] as string;
            this.categoryName = dr["categoryName"] as string;
        }
    }
}

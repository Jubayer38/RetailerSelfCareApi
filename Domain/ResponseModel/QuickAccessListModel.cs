namespace Domain.ResponseModel
{
    public class QuickAccessListModel
    {
        public List<QuickAccessModel> activeWidgetList { get; set; }
        public List<QuickAccessModel> inactiveWidgetList { get; set; }

        public QuickAccessListModel()
        {
            activeWidgetList = new List<QuickAccessModel>();

            inactiveWidgetList = new List<QuickAccessModel>();
        }
    }
}

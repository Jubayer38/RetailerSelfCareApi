using System.ComponentModel.DataAnnotations;

namespace Domain.Helpers
{
    public class StringDateTimeFormat : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is not DateTime)
                return false;

            DateTime currentDate = DateTime.Now;
            DateTime dateTime = (DateTime)value;
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;
            int hour = dateTime.Hour;

            if (year != currentDate.Year ||
                month != currentDate.Month ||
                day != currentDate.Day ||
                hour != currentDate.Hour)
                return false;

            return true;
        }
    }


    public class StringDateFormat : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is not DateTime)
                return false;

            DateTime currentDate = DateTime.Now;
            DateTime dateTime = (DateTime)value;
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;

            if (year != currentDate.Year ||
                month != currentDate.Month ||
                day != currentDate.Day)
                return false;

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BSIActivityManagement.Enum;

namespace BSIActivityManagement.Extensions
{

    

    public static class DisplayExtension
    {
        public static string DateToPersian(DateTime RequestedDate)
        {
            System.Globalization.PersianCalendar Taghvim = new System.Globalization.PersianCalendar();
            return Taghvim.GetDayOfWeek(RequestedDate).GetPersianDayofWeek()+" " + Taghvim.GetYear(RequestedDate) + "/" + Taghvim.GetMonth(RequestedDate) + "/" + Taghvim.GetDayOfMonth(RequestedDate);
        }

        public static string RemainingDays(DateTime DueDate, DateTime PaymentDate)
        {
            return DueDate > PaymentDate ? (DueDate - PaymentDate).Days.ToString() + "روز باقیمانده" : (PaymentDate - DueDate).Days.ToString() + "روز تاخیر";
        }

        public static string ElapsedTime(DateTime RequestedTime)
        {
            DateTime CurrentTime = DateTime.Now;

            if (RequestedTime > CurrentTime)
                return "آینده";

            TimeSpan Ts = CurrentTime - RequestedTime;

            if (Ts.Days == 0)
            {
                if (Ts.Hours > 0)
                    return Ts.Hours.ToString() + " ساعت پیش";

                if(Ts.Minutes > 0)
                    return Ts.Minutes.ToString() + " دقیقه پیش";
                
                return "اکنون";
            }

            if(Ts.Days < 31)
                return Ts.Days.ToString() + " روز پیش";

            return "بیش از یکماه پیش";

        }

        private static string GetPersianDayofWeek(this DayOfWeek EnglishWeekday)
        {
            switch (EnglishWeekday)
            {
                case DayOfWeek.Friday:
                    return "جمعه";
                case DayOfWeek.Saturday:
                    return "شنبه";
                case DayOfWeek.Sunday:
                    return "یک شنبه";
                case DayOfWeek.Monday:
                    return "دو شنبه";
                case DayOfWeek.Tuesday:
                    return "سه شنبه";
                case DayOfWeek.Wednesday:
                    return "چهار شنبه";
                case DayOfWeek.Thursday:
                    return "پنجشنبه";
                default:
                    return " ";
            }
        }
    }

    public static class HtmlExtensions
    {
        public static HtmlString CustomerEnumDisplayNameFor(this CustomerType item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString AccountEnumDisplayNameFor(this AccountType item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString PhoneEnumDisplayNameFor(this PhoneType item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString LoanEnumDisplayNameFor(this LoanType item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString LoanExtraInfoEnumDisplayNameFor(this LoanExtraInfoType item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString InstallmentEnumDisplayNameFor(this InstallmentStatus item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString InstallmentActionEnumDisplayNameFor(this InstallmentAction item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static HtmlString InstallmentNotificationActionEnumDisplayNameFor(this InstallmentNotificationAction item)
        {
            var type = item.GetType();
            var member = type.GetMember(item.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayName != null)
            {
                return new HtmlString(displayName.Name);
            }

            return new HtmlString(item.ToString());
        }

        public static DateTime PersianDateToDateTime(this DateTime GeorgianDate)
        {
            DateTime ResDateTime = new DateTime(1800, 1, 1);
            System.Globalization.PersianCalendar Taghvim = new System.Globalization.PersianCalendar();
            try
            {
                return Taghvim.ToDateTime(GeorgianDate.Year, GeorgianDate.Month, GeorgianDate.Day,1,1,1,1);
            }
            catch
            {
                return ResDateTime;
            }
        }
    }

}
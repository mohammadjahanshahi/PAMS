using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace BSIActivityManagement.Enum
{
    public enum QualityIndexEnum
    {
        [Display(Name = "افزایش تعداد")]
        IncreasingAmount = 1,
        [Display(Name = "کاهش تعداد")]
        DecreasingAmount = 2,
        [Display(Name = "افزایش مبلغ")]
        IncreasingPrice = 3,
        [Display(Name = "کاهش مبلغ")]
        DecreasingPrice = 4,
        [Display(Name = "اختصاص نمره")]
        Score = 5,
        [Display(Name = "بازه زمانی")]
        TimeDuration = 6
    }

    public enum CustomerType
    {
        [Display(Name = "مشتری حقیقی")]
        Person = 1,
        [Display(Name = "مشتری حقوقی")]
        Corporation = 2
    }

    public enum AccountType
    {
        [Display(Name = "حساب جاری")]
        SimpleAccount = 1,
        [Display(Name = "حساب کوتاه مدت")]
        ShortTermAccount = 2,
        [Display(Name = "حساب پس انداز")]
        SavingAccount = 3,
        [Display(Name = "حساب بلندمدت")]
        LongTermAccount = 4,
        [Display(Name = "حساب گنجینه")]
        TreasuryAccount = 8
    }

    public enum PhoneType
    {
        [Display(Name = "تلفن ثابت")]
        LandLine = 1,
        [Display(Name = "تلفن همراه")]
        Mobile = 2
    }

    public enum LoanType
    {
        [Display(Name = "نوع پیش فرض")]
        Default = 1,
        [Display(Name = "نوع پیش فرض میزان")]
        Mizan = 800,
        [Display(Name = "نوع ۲ میزان")]
        Mizan2 = 802,
        [Display(Name = "نوع 3 میزان")]
        Mizan3 = 803,
        [Display(Name = "نوع 7 میزان")]
        Mizan7 = 807,
        [Display(Name = "نوع 9 میزان")]
        Mizan9 = 809,
        [Display(Name = "نوع 11 میزان")]
        Mizan11  = 811,
        [Display(Name = "نوع 13 میزان")]
        Mizan13 = 813,
        [Display(Name = "فروش اقساطی میزان")]
        Mizan15 = 815,
        [Display(Name = "نوع 16 میزان")]
        Mizan16 = 816,
        [Display(Name = "نوع 17 میزان")]
        Mizan17 = 817,
        [Display(Name = "نوع 18 میزان")]
        Mizan18 = 818,
        [Display(Name = "نوع 19 میزان")]
        Mizan19 = 819,
        [Display(Name = "خرید دین میزان")]
        Mizan21 = 821,
        [Display(Name = "نوع 22 میزان")]
        Mizan22 = 822,
        [Display(Name = "مشارکت مدنی میزان")]
        Mizan27 = 827,
        [Display(Name = "نوع 39 میزان")]
        Mizan39 = 839,
        [Display(Name = "نوع 44 میزان")]
        Mizan44 = 844,
        [Display(Name = "نوع 45 میزان")]
        Mizan45 = 845,
        [Display(Name = "نوع 47 میزان")]
        Mizan47 = 847
    }

    public enum InstallmentStatus
    {
        [Display(Name = "پرداخت نشده")]
        Unpaid = 1,
        [Display(Name = "بخشی پرداخت شده")]
        PaidPartial = 2,
        [Display(Name = "پرداخت شده")]
        Paid = 3
    }

    public enum NotificationStatus
    {
        [Display(Name = "مشاهده نشده")]
        Unseen = 1,
        [Display(Name = "مشاهده شده")]
        Seen = 2
    }

    public enum MonthNumbers
    {
        [Display(Name = "یک ماهه")]
        one = 1,
        [Display(Name = "دو ماهه")]
        two = 2,
        [Display(Name = "سه ماهه")]
        three = 3,
        [Display(Name = "چهار ماهه")]
        four = 4,
        [Display(Name = "پنج ماهه")]
        five = 5,
        [Display(Name = "شش ماهه")]
        six = 6,
        [Display(Name = "هفت ماهه")]
        seven = 7,
        [Display(Name = "هشت ماهه")]
        eight = 8,
        [Display(Name = "نه ماهه")]
        nine = 9,
        [Display(Name = "ده ماهه")]
        ten = 10,
        [Display(Name = "یازده ماهه")]
        eleven = 11,
        [Display(Name = "دوازده ماهه")]
        twelve = 12
    }

    public enum InstallmentAction
    {
        [Display(Name = "تایید پرداخت")]
        SetAsPaid = 1,
        [Display(Name = "تعیین عدم پرداخت")]
        SetAsUnpaid = 2
    }

    public enum InstallmentNotificationAction
    {
        [Display(Name = "ایجاد یادآوری")]
        Create = 1,
        [Display(Name = "انجام یادآوری")]
        MakeAsRead = 2
    }

    public enum CorporationMemberType
    {
        [Display(Name = "شماره مشتری اصلی")]
        AsMain = 1,
        [Display(Name = "شماره مشتری اعضا")]
        AsMember = 2
    }

    public enum LoanExtraInfoType
    {
        [Display(Name = "شماره قرارداد سیستم صبا میزان")]
        SabaMizan = 1,
        [Display(Name = "شماره قرارداد سیستم سیمیا میزان")]
        SimiaMizan = 2,
        [Display(Name = "شماره قرارداد همکاران")]
        EmployeeMizan = 3,
        [Display(Name = "شماره قرارداد سنتی صادرات")]
        ContractNumber = 4,
        [Display(Name = "شماره قرارداد صبا سیمیا میزان")]
        SabaSimiaMizan = 5,
    }

    public enum MizanLoanCustomerType
    {
        [Display(Name = "وام گیرنده")]
        Debtor = 1,
        [Display(Name = "ضامن")]
        Referee = 2
    }

    public enum ConversionResultStatus
    {
        [Display(Name = "فاقد عملیات")]
        NotConverted = 0,
        [Display(Name = "موفقیت آمیز")]
        Success = 1,
        [Display(Name = "ناموفق")]
        Failed = 2,
        [Display(Name = "هشدار")]
        Warning = 3,
        [Display(Name = "ناممکن")]
        Impossible = 4,
        [Display(Name = "موجود از قبل")]
        AlreadyExist = 5,
        [Display(Name = "خطای قسط بندی")]
        InstallmentError = 6,
        [Display(Name = "خطای شعبه بانک صادرات")]
        BSIBranchError = 7,
        [Display(Name = "ضامن و متعهد نبود")]
        DebtorRefereeError = 8,
        [Display(Name = "متعهد شناسایی نشد")]
        DebtorError = 9,
        [Display(Name = "خطای تعداد اقساط")]
        InstallmentCountError = 10,
        [Display(Name = "خطای فاصله اقساط")]
        InstallmentPeriodError = 11,
        [Display(Name = "خطای نوع تسهیلات میزان")]
        MizanLoanTypeError = 12,
        [Display(Name = "خطای تاریخ اعطای تسهیلات")]
        LoanDateError = 13
    }

    public enum CustomerTypePersonalCorporation
    {
        [Display(Name = "حقیقی")]
        Person = 0,
        [Display(Name = "حقوقی")]
        Corporation = 1
    }

    public enum ForgotPasswordCodeStatus
    {
        [Display(Name = "استفاده نشده")]
        UnUsed = 0,
        [Display(Name = "استفاده شده")]
        Used = 1
    }

    public enum UpdateLoanStatus
    {   
        [Display(Name = "در حال پیگیری")]
        UnderProcess = 1,
        [Display(Name = "پایان پیگیری")]
        ProcessCompleted = 2,
        [Display(Name = "رها شده")]
        TimeOut = 0
    }

}
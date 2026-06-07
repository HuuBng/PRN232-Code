using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Validation
{
    public class FptStudentCodeAttribute : RegularExpressionAttribute
    {
        public FptStudentCodeAttribute() : base("^(SE|CE)\\d{5}$")
        {
            ErrorMessage = "Student code must match FPTU format, for example SE19886 or CE18793.";
        }
    }
}

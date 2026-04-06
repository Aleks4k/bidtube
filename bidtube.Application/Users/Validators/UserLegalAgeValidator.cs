using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Validators
{
    public static class UserLegalAgeValidator
    {
        public static bool IsLegal(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) //Vraćamo true jer user nije dužan da za vreme registracije šalje podatak o rođendanu.
                return true;
            if (DateTime.Today.AddYears(-16).CompareTo(dateOfBirth) < 0)
            {
                return false;
            }
            return true;
        }
    }
}

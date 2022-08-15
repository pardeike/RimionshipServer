using System.ComponentModel.DataAnnotations;

namespace RimionshipServer
{
    public class MustBeCheckedAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is bool b)
                return b;
            if (value == null)
                return false;

            throw new InvalidOperationException("Cannot be used on non-boolean types");
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace cinemaSystem.Validation
{
    //[AttributeUsage(AttributeTargets.Property)]
    public class ActorLengthAttribute : ValidationAttribute
    {
        private readonly int _maxLength;
        private readonly int _minLength;

        public ActorLengthAttribute(int minLength ,int maxLength)
        {
            _maxLength = maxLength;
            _minLength = minLength;
        }

        public override bool IsValid(object? value)
        {
            if(value is string result)
            {
                if(result.Length >=_minLength && result.Length <_maxLength)
                {
                    return true;
                }

            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The length of the  filed {name} must be between {_minLength} and {_maxLength} .";
        }


    }
}

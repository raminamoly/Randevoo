using Randevoo.Domain.Entities;

namespace Randevoo.Tests.Unit.Builder
{

    public class UserBuilder
    {
        string _mobileNumber = "+989121234567";

        public UserBuilder() { }
        public UserBuilder WithMobileNumber(string mobileNumber)
        {
            this._mobileNumber = mobileNumber;
            return this;
        }

        public User Build()
        {
            return new User(_mobileNumber);
        }

    }
}

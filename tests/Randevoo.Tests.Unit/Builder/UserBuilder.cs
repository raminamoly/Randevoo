using Randevoo.Domain.Entities;

namespace Randevoo.Tests.Unit.Builder
{

    public class UserBuilder
    {
        string _email = "Ramin.Amoly@gmail.com";
        string _password = "123";

        public UserBuilder() { }
        public UserBuilder WithEmail(string email)
        {
            this._email = email;
            return this;
        }
        public UserBuilder WithPassword(string password)
        {
            this._password = password;
            return this;
        }

        public User Build()
        {
            return new User(_email, _password);
        }

    }
}

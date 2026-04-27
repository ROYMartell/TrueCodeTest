using TrueCodeTest.Users.Contracts.Auth;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Mappers;

public static class UserMappings
{
    public static UserDto ToDto(this User user) => new(user.Id, user.Name);
}

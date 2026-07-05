using DevExpress.Xpo;
using Sln.Domain.Entities;
using Sln.Domain.DTOs;
using Sln.DataAccess.XpoEntities;

namespace Sln.DataAccess.Mappers;

public static class XpoUserMapper
{
    public static XpoUser ToXpo(User domain, UnitOfWork uow)
    {
        // Carica o crea l'oggetto XPO
        var xpo = uow.GetObjectByKey<XpoUser>(domain.Id) 
                  ?? new XpoUser(uow)
                  {
                      Id = domain.Id
                  };

        // Primitive fields
        xpo.Id            = domain.Id;
        xpo.Email         = domain.Email;
        xpo.PasswordHash  = domain.PasswordHash;
        xpo.PasswordSalt  = domain.PasswordSalt;
        xpo.Role          = domain.Role;

        return xpo;
    }

    public static User ToDomain(XpoUser xpo)
    {
        return new User(
            id: xpo.Id,
            email: xpo.Email,
            passwordHash: xpo.PasswordHash,
            passwordSalt: xpo.PasswordSalt,
            role: xpo.Role
        );
    }
        public static UserDto ToDto(User domain)
    {
        return new UserDto
        {
            Id = domain.Id,
            Email = domain.Email,
            Role = domain.Role
        };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs.Reponses;
using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.DTOs.Conversions
{
    public static class UserConversion
    {
       

        public static UserResponse? FormEntityToUserResponse(ApplicationUser? applicationUser, string? avatarUrl = null)
        {
            if (applicationUser is null)
                return null;

            return ConvertToUserResponse(applicationUser, avatarUrl);
        }

        private static UserResponse ConvertToUserResponse(ApplicationUser applicationUser, string? avatarUrl = null) => new()
        {
            Id = applicationUser.Id.ToString(),
            FullName = applicationUser.FullName,
            Email = applicationUser.Email,
            PhoneNumber = applicationUser.PhoneNumber,
            Address = applicationUser.Address,
            Avatar = avatarUrl,
            Role = applicationUser.Role.ToString()
        };
    }
}

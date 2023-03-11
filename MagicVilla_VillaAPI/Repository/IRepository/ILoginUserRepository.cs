using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Repository.IRepostiory
{
    public interface ILoginUserRepository
    {
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}

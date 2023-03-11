using AutoMapper;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicVilla_VillaAPI.data;

namespace MagicVilla_VillaAPI.Repository
{
    public class LoginLoginUserRepository : ILoginUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string _secretKey;
        private string _apiUser;
        private string _apiPassword;
        private readonly IMapper _mapper;

        public LoginLoginUserRepository(ApplicationDbContext db, IConfiguration configuration,
            IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _mapper = mapper;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _apiUser = configuration.GetValue<string>("ApiSettings:ApiUser");
            _apiPassword = configuration.GetValue<string>("ApiSettings:ApiPassword");
            _roleManager = roleManager;
        }

        
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            if (_apiUser != loginRequestDto.UserName
                || _apiPassword != loginRequestDto.Password)
            {
                return new LoginResponseDto();
            }
            
            //if user was found generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, loginRequestDto.UserName),
                    new Claim(ClaimTypes.Role, "default")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                Token = tokenHandler.WriteToken(token),
                
            };
            return loginResponseDto;
        }

    }
}

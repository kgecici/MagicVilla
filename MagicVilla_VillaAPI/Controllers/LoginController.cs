using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepostiory;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/login")]
[ApiController]
public class UsersController : ControllerBase
{
    private ApiResponse _response;
    private readonly ILoginUserRepository _loginUserRepo;
    public UsersController(ILoginUserRepository loginUserRepo)
    {
        _response = new();
        _loginUserRepo = loginUserRepo;
    }
    
    [HttpPost()]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        var loginResponse = await _loginUserRepo.Login(model);
        if (string.IsNullOrEmpty(loginResponse.Token))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_response);
        }
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);
    }

}
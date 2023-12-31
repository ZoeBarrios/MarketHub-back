﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.Auth;
using EcommerceAPI.Models.Auth.Dto;
using EcommerceAPI.Models.Role;
using EcommerceAPI.Models.Role.Dto;
using EcommerceAPI.Models.User;
using EcommerceAPI.Models.User.Dto;
using EcommerceAPI.Services;

namespace EcommerceAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IEncoderService _encoderService;
        private readonly AuthService _authService;
        private readonly RoleService _roleService;

        public AuthController(UserService userService, IEncoderService encoderService, AuthService authService, RoleService roleService, IMapper mapper)
        {
            _userService = userService;
            _encoderService = encoderService;
            _authService = authService;
            _roleService = roleService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (login.Email == null && login.UserName == null)
                {
                   
                    return BadRequest("Credentials are incorrect");
                }

                var user = await _userService.GetByUsernameOrEmail(login.UserName, login.Email);


                if (user == null || !_encoderService.Verify(login.Password, user.Password))
                {

                    return BadRequest("Credentials are incorrect");
                }

                var userResponse = new UserLoginResponseDto
                {
                    Id = user.UserId,
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = user.Roles.Select(p => p.Name).ToList()
                };
                string token = _authService.GenerateJwtToken(user);

                return Ok(new LoginResponseDto { Token = token, User = userResponse });

                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userService.GetByUsernameOrEmail(register.UserName, register.Email);

                if (user != null)
                {
                    
                    return BadRequest("User already exists");
                }

                var userCreated = await _userService.Create(register);

                var defaultRole = await _roleService.GetRoleByName("User");

                await _userService.UpdateUserRolesById(userCreated.UserId, new List<Role> { defaultRole });

                return Created("RegisterUser", userCreated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("roles/user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Put(int id, [FromBody] UpdateUserRolesDto updateUserRolesDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roles = await _roleService.GetRolesByIds(updateUserRolesDto.RoleIds);
                var userUpdated = await _userService.UpdateUserRolesById(id, roles);
                return Ok(userUpdated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

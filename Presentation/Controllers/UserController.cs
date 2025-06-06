﻿using App.Application.Commands.User;
using App.Application.Queries.User;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUserAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetAllUserQuery(pageSize, pageNumber));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin")]
        public async Task<IActionResult> AddAdminAsync(AddAdminDto addAdminDto)
        {
            var result = await sender.Send(new AddAdminCommand(addAdminDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchForUserAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchUserQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmailAsync([FromRoute] string email)
        {
            var result = await sender.Send(new GetUserByEmailQuery(email));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string email, [FromForm] UpdateUserRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateUserCommand(email, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string email)
        {
            var result = await sender.Send(new DeleteUserCommand(email));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

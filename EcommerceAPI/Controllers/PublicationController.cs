﻿using EcommerceAPI.Models.Publication.Dto;
using EcommerceAPI.Models.Role;
using EcommerceAPI.Models.User.Dto;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [Route("api/publications")]
    [ApiController]
    [Authorize]
    public class PublicationController : ControllerBase
    {
        private readonly PublicationService _publicationService;
        private readonly AuthService authService;

        public PublicationController(PublicationService publicationService, AuthService authService)
        {
            _publicationService = publicationService;
            this.authService = authService;
        }


        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PublicationsDto>>> Get()
        {
            return Ok(await _publicationService.GetAll());
        }

        [HttpGet("{CategoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PublicationsDto>>> GetByCategory(int CategoryId)
        {
            return Ok(await _publicationService.GetAllByCategory(CategoryId));
        }


        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublicationDto>> Get(int id)
        {
            try
            {
                return Ok(await _publicationService.GetById(id));
            }
            catch
            {
                return NotFound(new { message = $"No publication with Id = {id}" });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<PublicationDto>> Post([FromBody] CreatePublicationDto createPublicationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publicationCreated = await _publicationService.Create(createPublicationDto);

            return Created("PublicationCreated", publicationCreated);

        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<PublicationDto>> Put(int id, [FromBody] UpdatePublicationDto updatePublicationDto)
        {
            
            try
            {
                var updatedPublication = await _publicationService.UpdateById(id, updatePublicationDto);
                return Ok(updatedPublication);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using DatingApp.API.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // Anytime any of this controllers methods are called the LogUserActivity code is executed
    [Authorize]
    [Route("api/users/{userid}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userid, int id)
        {
            // Check if the user submitting this request is the current user that is passed into this method
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null){
                return NotFound();
            }

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userid, MessageForCreationDto messageForCreationDto)
        {
            // Check if the user submitting this request is the current user that is passed into this method
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userid;

            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);
            
            if(recipient == null)
            {
                return BadRequest("Could not find user");
            }

            var message = _mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);

            if(await _repo.SaveAll()){
                var messageToReturn = _mapper.Map<MessageForCreationDto>(message);
                return CreatedAtRoute("GetMessage", 
                new {userid, id = message.Id}, messageToReturn);
            }
            
            throw new Exception("Creating the message failed on save.");
        } 
    }
}
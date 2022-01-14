using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessageController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageController(IUserRepository userRepository , IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }
        [HttpPost]

        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto ){
            var username = User.GetUserName();
            if(username == createMessageDto.recipientUsername.ToLower()) return BadRequest("You Can't sent to youself");

            var sender = await _userRepository.GetUserByNameAsync(username);

            var recipient = await _userRepository.GetUserByNameAsync(createMessageDto.recipientUsername);

            if(recipient == null) return BadRequest("User Not found");
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                RecipientUserName = createMessageDto.recipientUsername,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllSync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest ("FAiled");
        }

        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.username = User.GetUserName();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);
             Response.AddPaginationHeaders(messages.CurrentPage, messages.TotalPages, messages.PageSize,messages.TotalCount);

             return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){

            var currentUserName= User.GetUserName();
            return Ok(await _messageRepository.GetMessageThread(currentUserName,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<MessageDto>> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await _messageRepository.GetUserMessage(id);

            if(message.RecipientUserName != username && message.SenderUsername != username) 
            return Unauthorized("Unauthorized");

            if(message.RecipientUserName == username) message.RecipientDeleted =true;
            if(message.SenderUsername == username) message.SenderDeleted =true;

            if(message.RecipientDeleted && message.SenderDeleted){
                _messageRepository.DeleteMessage(message);
            }
             if(await _messageRepository.SaveAllSync()) return Ok();
             
             return BadRequest("Failed");

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
       // private readonly IMessageRepository _messageR;
        private readonly IMapper _mapper;
        //private readonly IUserRepository _userRepository;
        private readonly PresenceTracker _tracker;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly IUnitOfWork _unitOfWork;
        public MessageHub(IUnitOfWork unitOfWork, IMapper mapper, 
        IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _unitOfWork = unitOfWork;
            _presenceHub = presenceHub;
            _tracker = tracker;
           // _userRepository = userRepository;
            _mapper = mapper;
           // _messageR = messageR;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext= Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
            var group = await AddToGroup(groupName);

           

            var messages = await _unitOfWork.messageRepository.GetMessageThread(Context.User.GetUserName(),otherUser);
           
            if(_unitOfWork.HasChanges()) 
                await _unitOfWork.Complete();
            //await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);

            await Clients.Caller.SendAsync("RecieveMessageThread", messages);
             await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            //await Groups.RemoveFromGroupAsync(Context.ConnectionId,)
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto){
            var username = Context.User.GetUserName();
            if(username == createMessageDto.recipientUsername.ToLower()) 
            throw new HubException("You Can't sent to youself");

            var sender = await _unitOfWork.userRepository.GetUserByNameAsync(username);

            var recipient = await _unitOfWork.userRepository.GetUserByNameAsync(createMessageDto.recipientUsername);

            if(recipient == null) throw new HubException("User Not found");
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                RecipientUserName = createMessageDto.recipientUsername,
                Content = createMessageDto.Content
            };
            var groupName= GetGroupName(sender.UserName,recipient.UserName);
             //var groupName= GetGroupName(message.SenderUsername,message.RecipientUserName);
            var group = await _unitOfWork.messageRepository.GetMessageGroup(groupName);
            if(group.Connections.Any(x=>x.UserName == recipient.UserName)){
                message.DateRead=DateTime.Now;
            }else
            {
                var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
                if(connections != null){
                   await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                   new{ username = sender.UserName, knownAs = sender.KnownAs});
                }
               
            }


           _unitOfWork.messageRepository.AddMessage(message);

            if(await _unitOfWork.Complete()) 
            {
               
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

       /*  private async Task<bool> AddToGroup(HubCallerContext context, string groupName)
        {
            var group = await _messageR.GetMessageGroup(groupName);
            var connection =  new Connection(context.User.GetUserName(),context.ConnectionId);

            if(group ==null){
                group = new Group(groupName);
                _messageR.AddGroup(group);
            }
            group.Connections.Add(connection)
           return await _messageR.SaveAllSync();
        } */
        //private async Task<bool> AddToGroup(string groupName)
         private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _unitOfWork.messageRepository.GetMessageGroup(groupName);
            
            var connection =  new Connection(Context.User.GetUserName(),Context.ConnectionId);

            if(group ==null){
                group = new Group(groupName);
               _unitOfWork.messageRepository.AddGroup(group);
            }
            else{
               var conn = group.Connections.FirstOrDefault(x=>x.UserName == Context.User.GetUserName());
               if(conn !=null)
                    group.Connections.Remove(conn);
            }
            group.Connections.Add(connection);
           if( await _unitOfWork.Complete()) return group;

           throw new HubException("Error in Addtogroup");
        }

        private async Task<Group> RemoveFromGroup(){
            // var connection = await _messageR.GetConnection(Context.ConnectionId);
            // if(connection !=null){
            //     _messageR.RemoveConnection(connection);
            // }
             
             var group = await _unitOfWork.messageRepository.GetGroupForConnection(Context.ConnectionId);

            var connection = group.Connections.FirstOrDefault(x=>x.ConnectionId == Context.ConnectionId);
            if(connection !=null){
               _unitOfWork.messageRepository.RemoveConnection(connection);
            }

            //  if(group !=null){
            //      group.Connections.Remove(connection);
            //  }
             if( await _unitOfWork.Complete()) return group;
                throw new HubException("Error in RemoveFromGroup");
        }
        private string GetGroupName(string caller, string other){

            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
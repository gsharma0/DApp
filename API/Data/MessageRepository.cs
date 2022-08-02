using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
           return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
            .Include(c=>c.Connections)
            .Where(c=>c.Connections.Any(x=>x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x=>x.Connections).FirstOrDefaultAsync(x=>x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query =  _context.Messages.OrderByDescending(x=>x.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)// Added for optimization
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m=>m.RecipientUserName == messageParams.username && !m.RecipientDeleted),
                "Outbox" => query.Where(m=>m.SenderUsername == messageParams.username && !m.SenderDeleted),
                _ => query.Where(m=>m.RecipientUserName == messageParams.username && !m.RecipientDeleted && m.DateRead == null) 
            };

           // var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider); optimization
            return await PagedList<MessageDto>.CreateAsync(query,messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
           var messages = await _context.Messages
           //.Include(u=>u.Sender).ThenInclude(u=>u.Photos)
           //.Include(u=>u.Recipient).ThenInclude(u=>u.Photos)
            .Where(m=>m.Recipient.UserName == currentUserName && m.Sender.UserName == recipientUserName && !m.RecipientDeleted
                    || m.Sender.UserName == currentUserName && m.Recipient.UserName == recipientUserName && !m.SenderDeleted)
            .OrderBy(m=>m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)//Added for optimization
            .ToListAsync();

            var unreadMessages = messages.Where(m=>m.DateRead ==null && 
            m.RecipientUserName ==currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;

                    Message message1 = _mapper.Map<Message>(message);
                    _context.Entry(message1).State = EntityState.Modified;
                }
                
                
               //await _context.SaveChangesAsync(); UOW
            }
            
            //var mess = _mapper.Map<IEnumerable<Message>>(messages);
           // _context.Messages = mess;
            return messages;
           // return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<Message> GetUserMessage(int id)
        {
          return await _context.Messages
          .Include(s=>s.Sender)
          .Include(r=>r.Recipient)
          .SingleOrDefaultAsync(x=>x.Id ==id);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        //commented due to UOF
        /* public async Task<bool> SaveAllSync()
        {
            return await _context.SaveChangesAsync() > 0;
        } */ 
    }
}
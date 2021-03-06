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

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query =  _context.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(m=>m.Recipient.UserName == messageParams.username && !m.RecipientDeleted),
                "Outbox" => query.Where(m=>m.Sender.UserName == messageParams.username && !m.SenderDeleted),
                _ => query.Where(m=>m.Recipient.UserName == messageParams.username && !m.RecipientDeleted && m.DateRead == null) 
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
           var messages = await _context.Messages
           .Include(u=>u.Sender).ThenInclude(u=>u.Photos)
           .Include(u=>u.Recipient).ThenInclude(u=>u.Photos)
            .Where(m=>m.Recipient.UserName == currentUserName && m.Sender.UserName == recipientUserName && !m.RecipientDeleted
                    || m.Sender.UserName == currentUserName && m.Recipient.UserName == recipientUserName && !m.SenderDeleted)
            .OrderBy(m=>m.MessageSent)
            .ToListAsync();

            var unreadMessages = messages.Where(m=>m.DateRead ==null && m.Recipient.UserName ==currentUserName).ToList();

            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
               await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<Message> GetUserMessage(int id)
        {
          return await _context.Messages
          .Include(s=>s.Sender)
          .Include(r=>r.Recipient)
          .SingleOrDefaultAsync(x=>x.Id ==id);
        }

        public async Task<bool> SaveAllSync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
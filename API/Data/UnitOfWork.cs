using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext dataContext, IMapper mapper)
        {
            _mapper = mapper;
            _dataContext = dataContext;
             _dataContext.ChangeTracker.AutoDetectChangesEnabled =true;
        }

        public IMessageRepository messageRepository => new MessageRepository(_dataContext, _mapper);

        public IUserRepository userRepository =>  new UserRepository(_dataContext,_mapper);

        public ILikesRepository likesRepository => new LikeRepository(_dataContext);

        public async Task<bool> Complete()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {

            return _dataContext.ChangeTracker.HasChanges();
        }
    }
}
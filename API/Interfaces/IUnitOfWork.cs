using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IMessageRepository messageRepository {get;}
        IUserRepository userRepository{get;}

        ILikesRepository likesRepository {get;}

        public Task<bool> Complete();

        public bool HasChanges();
    }
}
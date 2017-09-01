﻿using OnlineStore.Entity.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Business.Contracts
{
    public interface IUserService
    {
        List<User> GetAll(Expression<Func<User,bool>> predicate = null);
        User Get(Expression<Func<User,bool>> predicate);
        User Add(User entity);
        User Update(User entity);
        int Delete(User entity);
        string[] GetUserRoles(User user);

    }
}

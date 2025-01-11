﻿using Learning_Backend.Databases;
using Learning_Backend.Contracts;

namespace Learning_Backend.Repositories
{
    public class RepoWrapper : IRepoWrapper
    {
        private IUserRepo? user;
        private readonly object _lock = new object();
        private readonly LearningDatabase _learningDatabase;
        private readonly IConfiguration _configuration;

        public RepoWrapper(LearningDatabase learningDatabase, IConfiguration configuration)
        {
            _learningDatabase = learningDatabase;
            _configuration = configuration;
        }

        public IUserRepo User
        {
            get
            {
                if (user == null)
                {
                    lock (_lock)
                    {
                        if (user == null)
                        {
                            user = new UserRepository(_learningDatabase, _configuration);
                        }
                    }
                }
                return user;
            }
        }
    }
}
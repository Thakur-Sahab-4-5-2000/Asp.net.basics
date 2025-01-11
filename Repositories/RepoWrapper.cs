using Learning_Backend.Databases;
using Learning_Backend.Contracts;
using StackExchange.Redis;

namespace Learning_Backend.Repositories
{
    public class RepoWrapper : IRepoWrapper
    {
        private IUserRepo? user;
        private readonly object _lock = new object();
        private readonly LearningDatabase _learningDatabase;
        private readonly IConfiguration _configuration;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RepoWrapper(LearningDatabase learningDatabase, IConfiguration configuration, 
            IConnectionMultiplexer connectionMultiplexer)
        {
            _learningDatabase = learningDatabase;
            _configuration = configuration;
            _connectionMultiplexer = connectionMultiplexer;
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
                            user = new UserRepository(_learningDatabase, _configuration, _connectionMultiplexer);
                        }
                    }
                }
                return user;
            }
        }
    }
}

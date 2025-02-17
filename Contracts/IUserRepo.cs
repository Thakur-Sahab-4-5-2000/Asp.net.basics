﻿namespace Learning_Backend.Contracts
{
    using Learning_Backend.DTOS;
    public interface IUserRepo
    {
        public Task<ReturnValues<UserDTO>> GetUserById(int id);
        public Task<ReturnValues<UserDTO>> GetAllUsers();
        public Task<ReturnValues<UserDTO>> LoginRequest(LoginUserDTO model);
        public Task<ReturnValues<UserDTO>> RegisterUser(RegisterUserDTO model);
        public Task<ReturnValues<UserDTO>> UpdateUsers(UpdateUserDTO model, int userId);
    }
}

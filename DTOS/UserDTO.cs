namespace Learning_Backend.DTOS
{
    using Learning_Backend.Models.LearningDatabaseModels;

    public class UserDTO
    {
        public int Id { get; set; }


        public string? Name { get; set; }

        public string? Email { get; set; }


        public int? Role { get; set; }

        public static implicit operator UserDTO(Users user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Username,
                Email = user.Email,
                Role = user.RolesId
            };
        }

        ///// <summary>
        ///// Implicit conversion from List<Users> to List<UserDTO>
        ///// </summary>
        //public static implicit operator List<UserDTO>(List<Users> userList)
        //{
        //    if (userList == null)
        //        throw new ArgumentNullException(nameof(userList));

        //    return userList.Select(user => new UserDTO
        //    {
        //        Id = user.Id,
        //        Name = user.Username,
        //        Email = user.Email,
        //        Role = user.Role
        //    }).ToList();
        //}
    }
}

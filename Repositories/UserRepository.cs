namespace Learning_Backend.Repositories;
using Learning_Backend.Contracts;
using Learning_Backend.Databases;
using Learning_Backend.DTOS;
using Learning_Backend.Models.LearningDatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserRepository : IUserRepo
{
    private LearningDatabase learningDatabase;
    private IConfiguration configuration;
    public UserRepository(LearningDatabase _learningDatabase, IConfiguration _configuration)
    {
        learningDatabase = _learningDatabase;
        configuration = _configuration;
    }

    public async Task<ReturnValues<UserDTO>> LoginRequest(LoginUserDTO model)
    {
        ReturnValues<UserDTO> returnValues = new ReturnValues<UserDTO>();
        try
        {
            model.Validate();
            var res = await learningDatabase.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (res == null) {
                returnValues.StatusCode = 200;
                returnValues.Message = "User not found";
            }
            else
            {
                bool hashedPassword = model.Password.VerifyPassword(res.PasswordHash);
                if (hashedPassword)
                {
                    string token = GenerateToken(res.Username, res.Role.ToString());
                    returnValues.StatusCode = 200;
                    returnValues.Message = "User found";
                    returnValues.Token = token;
                }
                else
                {
                    returnValues.StatusCode = 200;
                    returnValues.Message = "User not found";
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return returnValues;
    }

    public async Task<ReturnValues<UserDTO>> RegisterUser(RegisterUserDTO model)
    {
        ReturnValues<UserDTO> returnValues = new ReturnValues<UserDTO>();
        try
        {
            model.Validate();

            var userExists = await learningDatabase.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (userExists != null)
            {
                returnValues.StatusCode = 200;
                returnValues.Message = "User already exists";
            }
            else
            {
                Users userModel = new Users();
                userModel.Username = model.Username;
                userModel.Email = model.Email;
                userModel.Role = model.Role;
                userModel.PasswordHash = model.PasswordHash.HashPassword();
                await learningDatabase.Users.AddAsync(userModel);
                await learningDatabase.SaveChangesAsync();
                returnValues.StatusCode = 200;
                returnValues.Message = "User created successfully";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return returnValues;
    }

    public async Task<ReturnValues<UserDTO>> GetUserById(int id)
    {
        ReturnValues<UserDTO> returnValues = new ReturnValues<UserDTO>();
        try
        {
            var res = await learningDatabase.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (res != null)
            {
                returnValues.StatusCode = 200;
                returnValues.Message = "User found";
                returnValues.Data = res;
            }
            else
            {
                returnValues.StatusCode = 404;
                returnValues.Message = "User not found";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return returnValues;
    }

    public async Task<ReturnValues<UserDTO>> GetAllUsers()
    {
        ReturnValues<UserDTO> returnValues = new ReturnValues<UserDTO>();
        try
        {
            var res = await learningDatabase.Users
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Name = user.Username,
                    Email = user.Email,
                    Role = user.Role
                })
                .ToListAsync();

            if (res.Count>0)  
            {
                returnValues.StatusCode = 200;
                returnValues.Message = "Users found";
                returnValues.DataArray = res;
            }
            else
            {
                returnValues.StatusCode = 404;
                returnValues.Message = "Users not found";
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching users.", ex);
        }
        return returnValues;
    }

    private string GenerateToken(string username, string role)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_KEYS")));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: "Shubham Thakur",  
            audience: "Janleba", 
            claims: claims,          
            expires: DateTime.Now.AddMinutes(30), 
            signingCredentials: signingCredentials 
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}

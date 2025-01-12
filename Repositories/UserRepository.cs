namespace Learning_Backend.Repositories;
using Learning_Backend.Contracts;
using Learning_Backend.Databases;
using Learning_Backend.DTOS;
using Learning_Backend.Models.LearningDatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class UserRepository : IUserRepo
{
    private readonly LearningDatabase learningDatabase;
    private readonly IConfiguration configuration;
    private readonly IConnectionMultiplexer connectionMultiplexer;
    public UserRepository(LearningDatabase _learningDatabase, IConfiguration _configuration, 
        IConnectionMultiplexer _connectionMultiplexer)
    {
        learningDatabase = _learningDatabase;
        configuration = _configuration;
        connectionMultiplexer = _connectionMultiplexer;
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
                    string token = GenerateToken(res.Username, res.RolesId.ToString(),res.Id.ToString());
                    returnValues.StatusCode = 200;
                    returnValues.Message = "User found";
                    returnValues.Token = token;
                    returnValues.ImagePath = res.ProfileImagePath;
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
                returnValues.StatusCode = 201;
                returnValues.Message = "User already exists";
            }
            else
            {
                Users userModel = new Users();
                userModel.Username = model.Username;
                userModel.Email = model.Email;
                userModel.PasswordHash = model.PasswordHash.HashPassword();

                var roleExists = await learningDatabase.Roles
                    .FirstOrDefaultAsync(role => role.Id == model.Role);
                if (roleExists == null)
                {
                    returnValues.StatusCode = 400;
                    returnValues.Message = "Role not found";
                    return returnValues;
                }
                userModel.RolesId = roleExists.Id;

                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(stream);
                    }
                    userModel.ProfileImagePath = "/uploads/" + uniqueFileName;
                }

                await learningDatabase.Users.AddAsync(userModel);
                await learningDatabase.SaveChangesAsync();
                returnValues.StatusCode = 201;
                returnValues.Message = "User created successfully";

                string emailSubject = configuration.GetValue<string>("EmailSetting:APP_NAME");
                string emailBody = configuration.GetValue<string>("EmailTemplates:UserRegistration");

                var db = connectionMultiplexer.GetDatabase();
                var emailTask = new { email = model.Email, subject = emailSubject, body = emailBody };
                await db.ListRightPushAsync("taskQueue", JsonSerializer.Serialize(emailTask));
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
                    Role = user.RolesId
                })
                .ToListAsync();

                if (res.Count > 0)
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

    public async Task<ReturnValues<UserDTO>> UpdateUsers(UpdateUserDTO model, int userId)
    {
        ReturnValues<UserDTO> returnValues = new ReturnValues<UserDTO>();
        try
        {
            model.Validate();

            var userExists = await learningDatabase.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (userExists == null)
            {
                returnValues.StatusCode = 404;
                returnValues.Message = "User Not Found";
                return returnValues;
            }

            bool passwordIsCorrect = model.OldPassword.VerifyPassword(userExists.PasswordHash);
            if (!passwordIsCorrect)
            {
                returnValues.Message = "Please enter the correct password";
                returnValues.StatusCode = 400;
                return returnValues;
            }

            userExists.Email = model.Email ?? userExists.Email;

            if (!string.IsNullOrEmpty(model.Password))
            {
                userExists.PasswordHash = model.Password.HashPassword();
            }

            if (model.ProfilePicture != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(path);

                var profilePictureInDB = userExists.ProfileImagePath?.Split("/")[2];
                if (!string.IsNullOrEmpty(profilePictureInDB))
                {
                    var totalFilePath = Path.Combine(path, profilePictureInDB);

                    if (File.Exists(totalFilePath))
                    {
                        File.Delete(totalFilePath);
                    }
                }

                var newProfileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);

                var newProfilePath = Path.Combine(path, newProfileName);
                using (var stream = new FileStream(newProfilePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }

                userExists.ProfileImagePath = "/uploads/" + newProfileName;
            }

            learningDatabase.Update(userExists);
            await learningDatabase.SaveChangesAsync();

            returnValues.Message = "User updated successfully";
            returnValues.StatusCode = 200;

        }
        catch (Exception ex)
        {
            returnValues.Message = "An error occurred while updating the user.";
            returnValues.StatusCode = 500;
            Console.WriteLine(ex);
        }

        return returnValues;
    }



    private string GenerateToken(string username, string role, string userId)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWT_KEYS")));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", userId),
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

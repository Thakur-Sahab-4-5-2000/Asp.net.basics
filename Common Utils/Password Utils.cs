public static class PasswordHasher
{
    public static string HashPassword(this string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool VerifyPassword( this string enteredPassword, string storedHashedPassword)
    {

        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
    }
}

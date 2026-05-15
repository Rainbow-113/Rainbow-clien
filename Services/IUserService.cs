using Rainbow.Models;

namespace Rainbow.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponse> LoginAsync(LoginDto loginDto);
        Task<UserProfileDto> GetProfileAsync(string token);

        Task<bool> UpdateProfileAsync(UserProfileDto userProfileDto, string token);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model, string token);

        Task<List<UserProfileDto>> GetAllUsersAsync(string token, string fullName = "");

        Task<bool> DeleteUserAsync(string id, string token);


    }
}

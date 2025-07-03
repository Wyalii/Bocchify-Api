namespace Bocchify_Api.Contracts
{
    public class UpdateProfile
    {
        public string NewUsername { get; set; } = string.Empty;
        public string NewAvatar { get; set; } = string.Empty;
    }
}
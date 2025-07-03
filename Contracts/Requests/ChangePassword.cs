namespace Bocchify_Api.Contracts
{
    public class ChangePassword
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
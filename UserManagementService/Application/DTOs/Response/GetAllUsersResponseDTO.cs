namespace UserManagementService.Application.DTOs.Response
{
    public class GetAllUsersResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string Role { get; set; }
    }
}

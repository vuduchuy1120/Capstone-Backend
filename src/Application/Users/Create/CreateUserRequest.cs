namespace Application.Users.Create;

public class CreateUserRequest
{
    public string Id { get; set; }
    public string Fullname { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}

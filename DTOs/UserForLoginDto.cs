namespace DotnetAPI.DTOs
{
    public partial class UserForLoginDto
    {
        public string Email { get; set; }

        public string Password { get; set; }




        //To handle the case where string values are nullable:
        public UserForLoginDto()
        {
            if (Email == null)
            {
                Email = "";
            }
            if (Password == null)
            {
                Password = "";
            }

        }

    }
}
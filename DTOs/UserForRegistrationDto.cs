namespace DotnetAPI.DTOs
{
    public partial class UserForRegistrationDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string? FirstName {get; set;} ="";
        public string? LastName {get; set;} ="";
        
        public string? Gender {get; set;} ="";
        
        public string? JobTitle {get; set;} ="";
        
        public string? Department {get; set;} ="";
        
        public decimal Salary {get; set;}

        
        


        //To handle the case where string values are nullable:
        public UserForRegistrationDto()
        {
            if (Email == null)
            {
                Email = "";
            }
            if (Password == null)
            {
                Password = "";
            }
            if (PasswordConfirm == null)
            {
                PasswordConfirm = "";
            }

        }

    }
}
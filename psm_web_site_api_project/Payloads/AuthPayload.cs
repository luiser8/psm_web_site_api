using System.Text.RegularExpressions;

namespace psm_web_site_api_project.Payloads;

public class LoginPayload
{
    public string? Correo { get; set; }
    public string? Contrasena { get; set; }
    public bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }
}
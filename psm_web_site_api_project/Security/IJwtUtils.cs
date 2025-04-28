using psm_web_site_api_project.Payloads;

public interface IJwtUtils
{
    string CreateToken(TokenPayload tokenDto);
    string RefreshToken(TokenPayload tokenDto);
}

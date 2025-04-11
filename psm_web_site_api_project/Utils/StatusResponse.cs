namespace psm_web_site_api_project.Services.StatusResponse;

public class StatusResponseDto
{
    public int Code { get; set; }
    public string? Message { get; set; }
}

public static class GetStatusResponse
{
    private const int CodeOk = 200;
    private const int CodeError = 400;
    private const string MessageOk = "exitosamente";
    private const string MessageError = "ha sido fallido";

    public static StatusResponseDto GetStatusResponses(bool response, string resource, string action)
    {
        if (response)
        {
            return new StatusResponseDto { Code = CodeOk, Message = $"{resource} {action} {MessageOk}" };
        } else {
            return new StatusResponseDto { Code = CodeError, Message = $"{resource} {action} {MessageError}" };
        }
    }
}

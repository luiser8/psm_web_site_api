namespace psm_web_site_api_project.Services.StatusResponse;

public class StatusResponseDto
{
    public int Code { get; set; }
    public string? Message { get; set; }
}

public static class GetStatusResponse
{
    public static int _CODE_OK = 200;
    public static int _CODE_ERROR = 400;
    public static string _MESSAGE_OK = "exitosamente";
    public static string _MESSAGE_ERROR = "ha sido fallido";

    public static StatusResponseDto GetStatusResponses(bool response, string resource, string action)
    {
        if (response)
        {
            return new StatusResponseDto { Code = _CODE_OK, Message = $"{resource} {action} {_MESSAGE_OK}" };
        } else {
            return new StatusResponseDto { Code = _CODE_ERROR, Message = $"{resource} {action} {_MESSAGE_ERROR}" };
        }
    }
}

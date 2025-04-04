using System.Text;

namespace psm_web_site_api_project.Utils.Md5utils;
public static class Md5utilsClass
{
    private static System.Security.Cryptography.MD5? _md5String;

    public static string GetMd5(string password)
    {
        _md5String = System.Security.Cryptography.MD5.Create();
        ASCIIEncoding encoding = new();
        StringBuilder stringBuilder = new();
        var stream = _md5String.ComputeHash(encoding.GetBytes(password));

        foreach (var t in stream)
            stringBuilder.Append($"{t:x2}");

        return stringBuilder.ToString();
    }
}
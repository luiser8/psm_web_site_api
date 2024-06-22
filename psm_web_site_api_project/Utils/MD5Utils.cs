using System.Text;

namespace psm_web_site_api_project.Utils.Md5utils;
public static class Md5utilsClass
{
    private static System.Security.Cryptography.MD5 md5String;

    public static string GetMD5(string password)
    {
        md5String = System.Security.Cryptography.MD5.Create();
        ASCIIEncoding encoding = new ASCIIEncoding();
        StringBuilder stringBuilder = new StringBuilder();
        byte[] stream = md5String.ComputeHash(encoding.GetBytes(password));

        for (int i = 0; i < stream.Length; i++) stringBuilder.AppendFormat("{0:x2}", stream[i]);
        return stringBuilder.ToString();
    }
}
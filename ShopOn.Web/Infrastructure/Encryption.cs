using System.Security.Cryptography;
using System.Text;

namespace ShopOn.Web.Infrastructure;

public static class Encryption
{
    public static string Encrypt(string strToEncrypt, string strKey)
    {
        try
        {
            using var tripleDes = TripleDES.Create();
            using var md5 = MD5.Create();
            byte[] byteHash = md5.ComputeHash(Encoding.ASCII.GetBytes(strKey));
            tripleDes.Key = byteHash;
            tripleDes.Mode = CipherMode.ECB;
            byte[] byteBuff = Encoding.ASCII.GetBytes(strToEncrypt);
            using var encryptor = tripleDes.CreateEncryptor();
            return Convert.ToBase64String(encryptor.TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

    public static string Decrypt(string strEncrypted, string strKey)
    {
        try
        {
            using var tripleDes = TripleDES.Create();
            using var md5 = MD5.Create();
            byte[] byteHash = md5.ComputeHash(Encoding.ASCII.GetBytes(strKey));
            tripleDes.Key = byteHash;
            tripleDes.Mode = CipherMode.ECB;
            byte[] byteBuff = Convert.FromBase64String(strEncrypted);
            using var decryptor = tripleDes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(byteBuff, 0, byteBuff.Length);
            return Encoding.ASCII.GetString(decrypted);
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }
}

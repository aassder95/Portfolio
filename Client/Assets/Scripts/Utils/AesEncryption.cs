using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class AesEncryption
{
    #region Fields
    static readonly byte[] m_key = Convert.FromBase64String("8A+9xBM+g9V636IAl7DxzAFoxFlxp++S7zQLOOpwcNA=");
    static readonly byte[] m_iv = Convert.FromBase64String("mnti7wY3p0nk+Qq5Sbu3rQ==");
    #endregion
    #region Control
    public static void Generate()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            string base64Key = Convert.ToBase64String(aes.Key);
            string base64IV = Convert.ToBase64String(aes.IV);

            Debug.Log("AES Key (Base64): " + base64Key);
            Debug.Log("AES Key Length: " + aes.Key.Length);
            Debug.Log("AES IV (Base64): " + base64IV);
            Debug.Log("AES IV Length: " + aes.IV.Length);
        }
    }

    public static async UniTask<string> Encrypt(string text)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = m_key;
            aes.IV = m_iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(cs))
                    await sw.WriteAsync(text);

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static async UniTask<string> Decrypt(string text)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = m_key;
            aes.IV = m_iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(text)))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
                return await sr.ReadToEndAsync();
        }
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class GZipCompression
{
    #region Control
    public static async UniTask<string> Compress(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        using (MemoryStream msi = new MemoryStream(bytes))
        using (MemoryStream mso = new MemoryStream())
        {
            using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                await msi.CopyToAsync(gs);

            return Convert.ToBase64String(mso.ToArray());
        }
    }

    public static async UniTask<string> Decompress(string data)
    {
        byte[] bytes = Convert.FromBase64String(data);
        using (MemoryStream msi = new MemoryStream(bytes))
        using (MemoryStream mso = new MemoryStream())
        {
            using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                await gs.CopyToAsync(mso);

            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
    #endregion
}

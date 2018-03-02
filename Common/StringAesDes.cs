using System;
using System.Security.Cryptography;
using System.Text;
namespace Common
{
    /// <summary>
    /// aes字符串加密解密类
    /// create by gloomy 2017-12-19 10:26:42
    /// </summary>
    public class StringAesDes
    {
        /// <summary>
        /// 加密方法 (加密密码需要16位字符串)
        /// </summary>
        /// <param name="toEncrypt">需要加密的字符串</param>
        /// <param name="key">加密key</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public string Encrypt(string toEncrypt, string key, string iv="                ")
        {
            var enStr = string.Empty;
            try
            {
                var keyArray = UTF8Encoding.UTF8.GetBytes(key);
                var ivArray = UTF8Encoding.UTF8.GetBytes(iv);
                var toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                var rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.Zeros;

                var cTransform = rDel.CreateEncryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                enStr= Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("StringAesDes Encrypt run err! toEncrypt:{0} key:{1} iv:{2} err: {3}",toEncrypt,key,iv,e);
            }
            return enStr;
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="toDecrypt">需要解密的字符串</param>
        /// <param name="key">加密key</param>
        /// <param name="iv">偏移量</param>
        /// <returns></returns>
        public string Decrypt(string toDecrypt, string key, string iv="                ")
        {
            var decStr = string.Empty;
            try
            {
                var keyArray = UTF8Encoding.UTF8.GetBytes(key);
                var ivArray = UTF8Encoding.UTF8.GetBytes(iv);
                var toEncryptArray = Convert.FromBase64String(toDecrypt);

                var rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.Zeros;

                var cTransform = rDel.CreateDecryptor();
                var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                decStr= UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception e)
            {
                Console.WriteLine("StringAesDes Decrypt run err! toDecrypt:{0} key:{1} iv:{2} err: {3}",toDecrypt,key,iv,e);
            }
            return decStr.Trim('\0');
        }
    }
}
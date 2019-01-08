using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Roguelike.Models;

namespace Roguelike.Services
{
    public class SaveLoadService
    {
        private static StreamReader reader;
        private static StreamWriter writer;

        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public static bool CheckIfGamesaveExists()
        {
            return File.Exists("save.rlgs");
        }

        public static void Save(PlayerTile save)
        {
            if (!CheckIfGamesaveExists())
            {
                File.Create("save.rlgs").Close();
            }
            writer = new StreamWriter("save.rlgs");
            string data = "";

            data += save.MonstersKilled.ToString() + "|";
            data += save.Lifes.ToString() + "|";
            data += save.Score.ToString();
            string encrypted = Encrypt(data);

            writer.Write(encrypted);
            writer.Close();
            Console.WriteLine("Data succesfully saved");
        }

        public static PlayerTile Load()
        {
            if (!CheckIfGamesaveExists())
            {
                return null;
            }
            try
            {
                reader = new StreamReader("save.rlgs");
            }catch(Exception)
            {
                return null;
            }
            string data = reader.ReadLine();
            reader.Close();
            string[] decrypted = Decrypt(data).Split('|');
            PlayerTile newPlayer = PlayerTile.LoadPlayer(
                int.Parse(decrypted[2]),
                int.Parse(decrypted[1]),
                int.Parse(decrypted[0]));
            return newPlayer;
        }

        public static void RemoveSaveFile()
        {
            File.Delete("save.rlgs");
        }

        public static string Encrypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}

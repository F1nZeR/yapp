﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace yapp.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const string MasterKey = "realworld";

        private readonly HMACSHA512 _hmac = new HMACSHA512(Encoding.UTF8.GetBytes("realworld"));

        public byte[] Hash(string password, byte[] salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password);

            var allBytes = new byte[bytes.Length + salt.Length];
            Buffer.BlockCopy(bytes, 0, allBytes, 0, bytes.Length);
            Buffer.BlockCopy(salt, 0, allBytes, bytes.Length, salt.Length);

            return _hmac.ComputeHash(allBytes);
        }
    }
}
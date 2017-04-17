﻿using System;
using System.Threading.Tasks;

namespace POEStash
{
    public interface IJsonProvider : IDisposable
    {
        Task<string> GetJson(string token);
    }
}

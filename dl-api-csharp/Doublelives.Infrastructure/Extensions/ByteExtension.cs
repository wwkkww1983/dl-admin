﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Doublelives.Infrastructure.Extensions
{
    public static class ByteExtension
    {
        public static byte[] GetBytes(this string str) => str.GetBytes(Encoding.UTF8);

        public static byte[] GetBytes(this string str, Encoding encoding) => encoding.GetBytes(str);
    }
}

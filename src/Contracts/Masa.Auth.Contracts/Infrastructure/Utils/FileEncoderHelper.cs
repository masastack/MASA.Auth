// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public class FileEncoderHelper
{
    public static Encoding GetTextFileEncodingType(byte[] buffer)
    {
        Encoding encoding = Encoding.Default;
        if (buffer.Length >= 3 && buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
        {
            encoding = Encoding.UTF8;
        }
        else if (buffer.Length >= 3 && buffer[0] == 254 && buffer[1] == 255 && buffer[2] == 0)
        {
            encoding = Encoding.BigEndianUnicode;
        }
        else if (buffer.Length >= 3 && buffer[0] == 255 && buffer[1] == 254 && buffer[2] == 65)
        {
            encoding = Encoding.Unicode;
        }
        else if (IsUTF8Bytes(buffer))
        {
            encoding = Encoding.UTF8;
        }
        return encoding;
    }

    private static bool IsUTF8Bytes(byte[] data)
    {
        int charByteCounter = 1;
        byte curByte;
        for (int i = 0; i < data.Length; i++)
        {
            curByte = data[i];
            if (charByteCounter == 1)
            {
                if (curByte >= 0x80)
                {
                    while (((curByte <<= 1) & 0x80) != 0)
                    {
                        charByteCounter++;
                    }
                    if (charByteCounter == 1 || charByteCounter > 6)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if ((curByte & 0xC0) != 0x80)
                {
                    return false;
                }
                charByteCounter--;
            }
        }
        if (charByteCounter > 1)
        {
            throw new Exception("unexpected byte format");
        }
        return true;
    }
}
﻿namespace Nein.Extensions.Exceptions;

public class NullOrEmptyException : Exception
{
    public NullOrEmptyException(string? message) : base(message)
    {
    }
}
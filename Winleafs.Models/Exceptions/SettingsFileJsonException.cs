﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Winleafs.Models.Exceptions
{
    [Serializable]
    public class SettingsFileJsonException : Exception
    {
        public SettingsFileJsonException()
        {
        }

        public SettingsFileJsonException(Exception e) : base("Error loading settings, corrupt JSON", e)
        {
        }

        public SettingsFileJsonException(string message) : base(message)
        {
        }

        public SettingsFileJsonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SettingsFileJsonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

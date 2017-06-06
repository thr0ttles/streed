using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace Streed.Models
{
    /// <summary>
    /// SerializableException takes a System.Exception,
    /// exposes the most used properties, and makes it
    /// serializable.
    /// </summary>
    [DataContract]
    public class SerializableException
    {
        public SerializableException()
        {
            Message = string.Empty;
            StackTrace = string.Empty;
            InnerException = null;
            Uri = string.Empty;
        }

        public SerializableException(SerializableException src)
        {
            if (src == null) return;

            Message = src.Message;
            StackTrace = src.StackTrace;
            InnerException = (src.InnerException == null ? null : new SerializableException(src.InnerException));
            Uri = src.Uri;
        }

        public SerializableException(string message)
        {
            Message = message;
        }

        public SerializableException(Exception src)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            Message = src.Message;
            StackTrace = src.StackTrace;
            if (src.InnerException != null)
            {
                InnerException = new SerializableException(src.InnerException);
            }
        }

        public SerializableException(Exception src, Uri uri) : this(src)
        {
            if (uri != null)
            {
                Uri = uri.ToString();
            }
        }

        public SerializableException(string message, Exception src)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            Message = message;
            StackTrace = src.StackTrace;
            InnerException = new SerializableException(src);
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public SerializableException InnerException { get; set; }

        [DataMember]
        public string Uri { get; set; }
    }
}

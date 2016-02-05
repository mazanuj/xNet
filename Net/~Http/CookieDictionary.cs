using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace xNet.Net
{
    /// <summary>
    /// Представляет коллекцию HTTP-куки.
    /// </summary>
    [Serializable]
    public class CookieDictionary : Dictionary<string, string>
    {
        /// <summary>
        /// Возвращает или задает значение, указывающие, закрыты ли куки для редактирования
        /// </summary>
        /// <value>Значение по умолчанию — <see langword="false"/>.</value>
        public bool IsLocked { get; private set; }


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CookieDictionary"/>.
        /// </summary>
        /// <param name="isLocked">Указывает, закрыты ли куки для редактирования.</param>
        public CookieDictionary(bool isLocked = false) : base(StringComparer.OrdinalIgnoreCase)
        {
            IsLocked = isLocked;
        }

	    ///<summary>
	    /// Конструктор для десериализации объекта
	    /// </summary>
	    protected CookieDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
	    {
	    }

	    /// <summary>
        /// Возвращает строку, состоящую из имён и значений куки.
        /// </summary>
        /// <returns>Строка, состоящая из имён и значений куки.</returns>
        override public string ToString()
        {
            var strBuilder = new StringBuilder();        

            foreach (var cookie in this)
            {
                strBuilder.AppendFormat("{0}={1}; ", cookie.Key, cookie.Value);
            }

            if (strBuilder.Length > 0)
            {
                strBuilder.Remove(strBuilder.Length - 2, 2);
            }

            return strBuilder.ToString();
        }
    }
}
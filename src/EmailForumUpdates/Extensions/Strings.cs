using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailForumUpdates.Extensions
{
    static class Strings
    {
        /// <summary>
        /// Strip out html tags and truncate to a maximum length of chars
        /// If it is truncating a word, it will look to see where the last word ended
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <param name="maxCharLen"></param>
        /// <returns></returns>
        public static string StripTruncateHtml(this string htmlStr, int maxCharLen)
        {
            var returnVar = Regex.Replace(htmlStr, "<[^>]*(>|$)", "").Trim();
            returnVar = Regex.Replace(returnVar, @"[\s\r\n]+", " ");
            // in terms of where to truncate without cutting off words, an interesting resource at http://www.ravi.io/language-word-lengths
            if (maxCharLen > returnVar.Length)
            {
                return returnVar;
            }
            int spacePos = returnVar.LastIndexOf(' ', maxCharLen);
            if (spacePos == -1)
            {
                return returnVar.Substring(0, maxCharLen - 1) + '…';
            }
            return returnVar.Substring(0, spacePos) + '…';
        }
    }
}

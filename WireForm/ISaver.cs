using System;
using System.Collections.Generic;
using System.Text;

namespace Wireform
{
    public interface ISaver
    {
        /// <summary>
        /// Saves the specified json string to the location specified by the identifier.
        /// If the identifier is "", prompt the user for where to save the data.
        /// NOTE: return "" if saving was canceled or failed.
        /// </summary>
        /// <returns>an arbitrary identifier which can be used to save to the same location. 
        /// Eg. On a local filesystem, the identifier could be the path of the file to be saved</returns>
        public string WriteJson(string json, string locationIdentifier);

        /// <summary>
        /// Possibly prompts the user for where to load and returns the json string at that location.
        /// NOTE: return "" if loading was canceled or failed.
        /// </summary>
        /// <returns>json string</returns>
        public string GetJson();
    }
}

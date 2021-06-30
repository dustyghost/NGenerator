using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NGenerator.Models
{
    public class InputHolder
    {
        public string Template { get; set; }
        public Dictionary<string, string[]> Tags { get; set; } = new Dictionary<string, string[]>();

        public string ToJson() => JsonConvert.SerializeObject(this);

        public bool SetFromJson(string input)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<InputHolder>(input);
                Template = result.Template;
                Tags = result.Tags;
            }
            catch (Exception ex)
            {
                Console.Write("Error converting JSON to input holder" + ex.Message);
                return false;
            }
            return true;
        }
    }
}
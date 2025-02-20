using Newtonsoft.Json;
using System.Text;

namespace WebAPITemplate.Helpers
{
    public class PatchContent : StringContent
    {
        public PatchContent(object value)
             : base(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json-patch+json")
        {
        }
    }
}

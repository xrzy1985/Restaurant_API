using Restaurant_API.response;

namespace Restaurant_API.queries
{
    public class ParamaterCheck
    {
        public ParamaterCheck() { }

        public Boolean IsMalicious(string param)
        {
            return param.Contains(";") || param.Contains("drop") || string.IsNullOrEmpty(param) ? true : false;
        }
    }
}

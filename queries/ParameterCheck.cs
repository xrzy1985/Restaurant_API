namespace Restaurant_API.queries
{
    public class ParameterCheck
    {
        public ParameterCheck() { }

        public Boolean IsMalicious(string param)
        {
            return param.Contains(";") || param.Contains("drop table") || string.IsNullOrEmpty(param) ? true : false;
        }
    }
}

namespace Mvc_Schedule.Models.DataModels.ModelViews
{
    public class Model4Views<T> where T : class, new()
    {
        public Model4Views() : this("", new T()) { }

        public Model4Views(string errormsg, T data)
        {
            this.ErrorMsg = errormsg;
            this.Data = data;
        }
        public string ErrorMsg { get; set; }
        public T Data { get; set; }
    }
}

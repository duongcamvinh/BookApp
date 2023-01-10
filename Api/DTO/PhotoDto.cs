using Castle.DynamicProxy.Generators.Emitters;

namespace Api.DTO
{
    public class PhotoDto
    {
        public int Id { get; set; } 
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}

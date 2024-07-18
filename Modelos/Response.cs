
namespace Cambios.Modelos
{
    public class Response   //Classe para detetar se está tudo bem
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }
    }
}

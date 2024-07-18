

namespace Cambios.Servicos
{
    using Modelos;
    using System.Net;

    public class NetworkService
    {
        public Response CheckConnection()
        {
            var client = new WebClient();   // é o que vai testar para saber se tem ou não acesso à internet

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))   //Pingamos o google para saber se temos ou não internet
                {
                    return new Response
                    {
                        IsSuccess = true
                    };
                }
            }
            catch  //Caso não consiga fazer a ligação
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Configure a sua ligação à Internet"
                };
                
            }
        }
    }
}

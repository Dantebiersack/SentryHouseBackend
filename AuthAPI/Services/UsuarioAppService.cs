using MongoDB.Driver;
using SentryHouseBackend.Models;

namespace SentryHouseBackend.Services
{
    public class UsuarioAppService
    {
        private readonly IMongoCollection<UsuarioApp> _usuarioAppCollection;

        public UsuarioAppService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MONGODB_URI"]);
            var database = client.GetDatabase("SentryHouseGas");
            _usuarioAppCollection = database.GetCollection<UsuarioApp>("usuario_app");
        }

        public async Task<bool> CrearUsuarioAppAsync(UsuarioApp usuario)
        {
            var existe = await _usuarioAppCollection.Find(u => u.Correo == usuario.Correo).FirstOrDefaultAsync();

            if (existe != null)
                return false;

            await _usuarioAppCollection.InsertOneAsync(usuario);
            return true;
        }

    }
}

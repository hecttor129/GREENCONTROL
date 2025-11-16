using BLL;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace CONTROLLER
{
    public class N8nSiembraController
    {

        private readonly SiembraService siembraService;
        private readonly PrediccionesSiembraService prediccionesSiembraService;
        private readonly HttpClient httpClient;
        public N8nSiembraController()
        {
            siembraService = new SiembraService();
            prediccionesSiembraService = new PrediccionesSiembraService();
            httpClient = new HttpClient();
        }

        public object GenerarJsonParaN8n(int idSiembra)
        {
            // Crear JSON de la siembra actual
            string jsonSiembra = prediccionesSiembraService.CrearJsonPrediccion(idSiembra);

            // Obtener el nombre del cultivo de la siembra
            var siembra = siembraService.ObtenerPorId(idSiembra.ToString());
            string nombreCultivo = siembraService.ObtenerNombreCultivo(siembra.IdSiembra);

            // Crear JSON del historial filtrando por cultivo
            string jsonHistorial = prediccionesSiembraService.CrearJsonHistoriales(nombreCultivo);

            // Devolver ambos JSONs juntos en un objeto
            return new
            {
                jsonSiembra,
                jsonHistorial
            };

        }

        public async Task<string> EnviarPrediccionAN8nAsync(int idSiembra)
        {
            try
            {
                // 1️⃣ Obtener JSONs
                var jsons = GenerarJsonParaN8n(idSiembra);

                // 2️⃣ Combinar en un solo payload
                var payload = new
                {
                    siembraActual = JsonSerializer.Deserialize<object>(jsons.GetType().GetProperty("jsonSiembra").GetValue(jsons).ToString()),
                    historiales = JsonSerializer.Deserialize<object>(jsons.GetType().GetProperty("jsonHistorial").GetValue(jsons).ToString())
                };

                string jsonPayload = JsonSerializer.Serialize(payload);      

                // 3️⃣ Enviar POST a n8n
                string n8nUrl = "https://TU-N8N-ENDPOINT"; // <-- reemplaza con tu URL real
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(n8nUrl, content);
                response.EnsureSuccessStatusCode();

                // 4️⃣ Leer respuesta
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return jsonResponse;
            }
            catch (Exception ex)
            {
                return $"Error al enviar a n8n: {ex.Message}";
            }
        }





    }
}






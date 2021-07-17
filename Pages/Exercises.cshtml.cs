using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CalisthenicsAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CalisthenicsWebClient.Pages
{
    public class ExercisesModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IEnumerable<ExerciseReadDto> Exercises { get; private set; }
        public bool GetExercisesError { get; private set; }

        public int SelectedExerciseId { get; set; }

        public ExercisesModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "exercises");

            var client = _clientFactory.CreateClient("calisthenics");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                Exercises = await JsonSerializer.DeserializeAsync
                    <IEnumerable<ExerciseReadDto>>(responseStream);
            }
            else
            {
                GetExercisesError = true;
                Exercises = Array.Empty<ExerciseReadDto>();
            }
        }
    }
}

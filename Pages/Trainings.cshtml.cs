using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CalisthenicsDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CalisthenicsWebClient.Pages
{
    public class TrainingsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IEnumerable<TrainingReadDto> Trainings { get; private set; }

        [BindProperty]
        public TrainingCreateDto NewTraining { get; set; }

        public bool GetTrainingsError { get; private set; }
        public bool PostCreateNewTrainingError { get; private set; }

        public TrainingsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGet()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "trainings");
            var client = _clientFactory.CreateClient("calisthenics");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                Trainings = await JsonSerializer.DeserializeAsync
                    <IEnumerable<TrainingReadDto>>(responseStream);
            }
            else
            {
                GetTrainingsError = true;
                Trainings = Array.Empty<TrainingReadDto>();
            }
        }

        public async Task<IActionResult> OnPostCreate()
        {
            var content = new StringContent(
                JsonSerializer.Serialize(NewTraining),
                Encoding.UTF8,
                "application/json");

            var client = _clientFactory.CreateClient("calisthenics");
            using var httpResponse = await client.PostAsync("trainings", content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                PostCreateNewTrainingError = true;
                return RedirectToPage("/Error");
            }

            return RedirectToPage("/Trainings");
        }
    }
}

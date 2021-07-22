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
    public class ExercisesModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IEnumerable<ExerciseReadDto> Exercises { get; private set; }

        [BindProperty]
        public ExerciseCreateDto NewExercise { get; set; }

        [BindProperty]
        public ExerciseUpdateDto UpdateExercise { get; set; }

        [BindProperty]
        public int SelectedExerciseId { get; set; }

        public bool GetExercisesError { get; private set; }      
        public bool PostNewExerciseError { get; private set; }
        public bool PostEditExerciseError { get; private set; }
        public bool PostDeleteExerciseError { get; private set; }

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

        public async Task<IActionResult> OnPost()
        {
            var content = new StringContent(
                JsonSerializer.Serialize(NewExercise),
                Encoding.UTF8,
                "application/json");

            var client = _clientFactory.CreateClient("calisthenics");
            using var httpResponse = await client.PostAsync("exercises", content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                PostNewExerciseError = true;
                return RedirectToPage("/Error");
            }

            return RedirectToPage("/Exercises");
        }

        public async Task<IActionResult> OnPostEdit()
        {
            var content = new StringContent(
                JsonSerializer.Serialize(UpdateExercise),
                Encoding.UTF8,
                "application/json");

            var client = _clientFactory.CreateClient("calisthenics");
            using var httpResponse = await client.PutAsync($"exercises/{SelectedExerciseId}", content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                PostEditExerciseError = true;
                return RedirectToPage("/Error");
            }

            return RedirectToPage("/Exercises");
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var client = _clientFactory.CreateClient("calisthenics");
            using var httpResponse = await client.DeleteAsync($"exercises/{SelectedExerciseId}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                PostDeleteExerciseError = true;
                return RedirectToPage("/Error");
            }

            return RedirectToPage("/Exercises");
        }
    }
}

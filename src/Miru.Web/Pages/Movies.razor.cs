using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Miru.Web.Models;

namespace Miru.Web.Pages;

public partial class Movies : ComponentBase
{
    private MovieModel[]? Items { get; set; }

    [Inject]
    public HttpClient? Http { get; set; }

    [Inject]
    public NavigationManager? NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Http is not null && NavigationManager is not null)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive =  true,
            };
            
            Items = await Http.GetFromJsonAsync<MovieModel[]>($"{NavigationManager.BaseUri}/sample-data/movie.json", options);
        }
    }
}
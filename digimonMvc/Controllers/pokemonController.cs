using digimonMvc.Helpers;
using digimonMvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
using System.Reflection;
using NuGet.Protocol.Core.Types;
using NToastNotify;

namespace digimonMvc.Controllers
{
    public class pokemonController : Controller
    {
        private readonly ILogger<pokemonController> _logger;

        string baseURL = "https://localhost:7284/api/";
        
        List<pokemonModel> pokemons = new List<pokemonModel>();

        private readonly IToastNotification _toastNotification;

        public pokemonController(ILogger<pokemonController> logger, IToastNotification toastNotification)
        {
            _logger = logger;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            IList<pokemonModel> pokemon = new List<pokemonModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage getResp = await client.GetAsync("pokemon/getPokemons");

                if ( getResp.IsSuccessStatusCode)
                {
                    string datat = getResp.Content.ReadAsStringAsync().Result;
                   
                    responseModel resp = JsonConvert.DeserializeObject<responseModel>(datat);

                    foreach (pokemonModel item in resp.detalle)
                    {
                        pokemonModel pokMod = new pokemonModel();

                        pokMod.Id = item.Id;
                        pokMod.name = item.name;
                        pokMod.season = item.season;
                        pokMod.partner = item.partner;
                        pokMod.imageUrl = item.imageUrl;

                        pokemon.Add(pokMod);
                    }
                    
                    

                }
                else
                {
                    Console.WriteLine("Error to consume api/pokemon/getPokemons!");
                }
                ViewData.Model = pokemon;
            }

            return View();
        }
        public async Task<IActionResult> update(int id)
        {
            updateModel obj = new updateModel();
            if (id > 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL + "pokemon/getPokemonById");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage getResp = await client.GetAsync($"?pokemonId={id}");

                    if (getResp.IsSuccessStatusCode)
                    {
                        string datat = getResp.Content.ReadAsStringAsync().Result;

                        responseModel resp = JsonConvert.DeserializeObject<responseModel>(datat);

                        obj.Id = resp.detalle[0].Id;
                        obj.name = resp.detalle[0].name;
                        obj.season = resp.detalle[0].season;
                        obj.partner = resp.detalle[0].partner;
                        obj.imageUrl = resp.detalle[0].imageUrl;
                    }
                    else
                    {
                        Console.WriteLine("Error to consume api/pokemon/getPokemonById!");
                    }
                    return View(obj);
                }
            }
            return View();
        }
        public async Task<ActionResult<String>> savePokemon(string namePokemon, pokemonModel pokemon)
        {
            pokemonModel obj = new pokemonModel()
            {
                Id = pokemon.Id,
                name = pokemon.name,
                season = pokemon.season,
                partner = pokemon.partner,
                imageUrl = pokemon.imageUrl,
            };

            if (pokemon.name != "")
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL + "pokemon/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage getResp = await client.PostAsJsonAsync<pokemonModel>("savePokemon", obj);

                    if (getResp.IsSuccessStatusCode)
                    {
                        string datat = getResp.Content.ReadAsStringAsync().Result;

                        responseModel resp = JsonConvert.DeserializeObject<responseModel>(datat);
                        _toastNotification.AddSuccessToastMessage(resp.mensaje);
                        return RedirectToAction("Index","Pokemon");
                        
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Error to consume api/pokemon/savePokemon and save Pokemon Data!");
                    }
                    
                }
            }
            return View();
        }
        public async Task<ActionResult<String>> updatePokemon(updateModel pokemon)
        {
            pokemonModel obj = new pokemonModel()
            {
                name = pokemon.namePokemon,
                season = pokemon.season,
                partner = pokemon.partner,
                imageUrl = pokemon.imageUrl,
            };

            if (pokemon.namePokemon.Length > 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL + "pokemon/updatePokemonData/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage getResp = await client.PutAsJsonAsync($"?namePokemon={pokemon.name}", pokemon); 

                    if (getResp.IsSuccessStatusCode)
                    {
                        string datat = getResp.Content.ReadAsStringAsync().Result;

                        responseModel resp = JsonConvert.DeserializeObject<responseModel>(datat);
                        _toastNotification.AddSuccessToastMessage(resp.mensaje);
                        return RedirectToAction("Index","Pokemon");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Error to Update Pokemon Data!");
                    }
                    
                }
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult redirect()
        {
            return RedirectToAction("savePokemon","Pokemon");
        }
    }
}
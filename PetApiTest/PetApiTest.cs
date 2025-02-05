﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using PetApi.Models;
using PetApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using System.Net.Mime;
using PetApi.Controllers;
using Microsoft.AspNetCore.Http;

namespace PetApiTest
{
    [Collection("Sequential")]
    public class PetApiTest
    {
        [Fact]
        public async void Should_add_new_pet_successful()
        {
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();

            var pet = new Pet(name: "Milu", type: "dog", color: "red", price: 100);
            var httpContent = new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("/Pet", httpContent);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_pets_when_create_pets()
        {
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var petController = new PetController();
            var pet = new Pet(name: "Milu", type: "dog", color: "red", price: 100);
            petController.Reset();
            var httpContent = new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent);
            await client.PostAsync("/Pet", httpContent);

            var response = await client.GetAsync("/Pet");
            var body = await response.Content.ReadAsStringAsync();

            var pets = JsonConvert.DeserializeObject<List<Pet>>(body);

            Assert.Equal(2, pets.Count);
            Assert.Equal(pet, pets[0]);
            Assert.Equal(pet, pets[1]);
        }

        [Fact]
        public async void Should_find_pet_when_give_name()
        {
            var petController = new PetController();
            petController.Reset();
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var pet = new Pet(name: "Milu", type: "dog", color: "red", price: 100);
            var httpContent = new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent);

            var response = await client.GetAsync("/Pet/Milu");
            var body = await response.Content.ReadAsStringAsync();
            var findPet = JsonConvert.DeserializeObject<Pet>(body);
            Assert.Equal(pet, findPet);
        }

        [Fact]
        public async void Should_remove_pet_when_give_name()
        {
            var petController = new PetController();
            petController.Reset();
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var pet = new Pet(name: "Milu", type: "dog", color: "red", price: 100);
            var httpContent = new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent);

            await client.DeleteAsync("/Pet/Milu");

            Assert.Empty(petController.Get());
        }

        [Fact]
        public async void Should_update_pet_price_by_pet_name_when_give_name()
        {
            var petController = new PetController();
            petController.Reset();
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var pet = new Pet(name: "Ali", type: "dog", color: "red", price: 100);
            var httpContent = new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent);

            var response = await client.PutAsync("/Pet/Ali?price=200", null);
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(200, JsonConvert.DeserializeObject<Pet>(body).Price);
        }

        [Fact]
        public async void Should_find_pets_when_given_type()
        {
            var petController = new PetController();
            petController.Reset();
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var pet = new Pet(name: "Ali", type: "cat", color: "red", price: 100);
            var pet2 = new Pet(name: "MiLu", type: "cat", color: "red", price: 100);
            var pet3 = new Pet(name: "Duoduo", type: "dog", color: "white", price: 100);
            var httpContent = new StringContent(JsonConvert.SerializeObject(pet), Encoding.UTF8, MediaTypeNames.Application.Json);
            var httpContent2 = new StringContent(JsonConvert.SerializeObject(pet2), Encoding.UTF8, MediaTypeNames.Application.Json);
            var httpContent3 = new StringContent(JsonConvert.SerializeObject(pet3), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent);
            await client.PostAsync("/Pet", httpContent2);
            await client.PostAsync("/Pet", httpContent3);

            var response = await client.GetAsync("/Pet/type?type=cat");
            var body = await response.Content.ReadAsStringAsync();
            var pets = JsonConvert.DeserializeObject<List<Pet>>(body);

            Assert.Equal(2, pets.Count);
            Assert.Equal(pet, pets[0]);
            Assert.Equal(pet2, pets[1]);
        }

        [Fact]
        public async void Should_find_pets_by_price_range()
        {
            var petController = new PetController();
            petController.Reset();
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var pet1 = new Pet(name: "Ali", type: "dog", color: "red", price: 100);
            var pet2 = new Pet(name: "MiLu", type: "cat", color: "red", price: 200);
            var pet3 = new Pet(name: "Duoduo", type: "dog", color: "white", price: 300);
            var httpContent1 = new StringContent(JsonConvert.SerializeObject(pet1), Encoding.UTF8, MediaTypeNames.Application.Json);
            var httpContent2 = new StringContent(JsonConvert.SerializeObject(pet2), Encoding.UTF8, MediaTypeNames.Application.Json);
            var httpContent3 = new StringContent(JsonConvert.SerializeObject(pet3), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent1);
            await client.PostAsync("/Pet", httpContent2);
            await client.PostAsync("/Pet", httpContent3);

            var response = await client.GetAsync("/Pet/price?minPrice=150&maxPrice=250");
            var body = await response.Content.ReadAsStringAsync();
            var pets = JsonConvert.DeserializeObject<List<Pet>>(body);

            Assert.Equal(pet2, pets[0]);
        }

        // write a test can find pets by color
        [Fact]
        public async void Should_find_pets_by_color()
        {
            var petController = new PetController();
            petController.Reset();
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient client = server.CreateClient();
            var pet1 = new Pet(name: "Ali", type: "dog", color: "red", price: 100);
            var pet2 = new Pet(name: "MiLu", type: "cat", color: "red", price: 200);
            var pet3 = new Pet(name: "Duoduo", type: "dog", color: "white", price: 300);
            var httpContent1 = new StringContent(JsonConvert.SerializeObject(pet1), Encoding.UTF8, MediaTypeNames.Application.Json);
            var httpContent2 = new StringContent(JsonConvert.SerializeObject(pet2), Encoding.UTF8, MediaTypeNames.Application.Json);
            var httpContent3 = new StringContent(JsonConvert.SerializeObject(pet3), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/Pet", httpContent1);
            await client.PostAsync("/Pet", httpContent2);
            await client.PostAsync("/Pet", httpContent3);

            var response = await client.GetAsync("/Pet/color?color=red");
            var body = await response.Content.ReadAsStringAsync();
            var pets = JsonConvert.DeserializeObject<List<Pet>>(body);

            Assert.Equal(2, pets.Count);
            Assert.Equal(pet1, pets[0]);
            Assert.Equal(pet2, pets[1]);
        }
    }
}

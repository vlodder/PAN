using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PAN.context.Models;
using PAN.Models;
using PAN.Services;
using Xunit;

namespace PAN.tests
{
    public class EvenementServiceTests
    {
        // Méthode utilitaire pour générer un contexte avec des données de test
        // On utilise un nom de base de données unique (Guid) pour que les tests soient isolés
        private async Task<GeipanContext> GetInMemoryContextAsync(string dbName)
        {
            var options = new DbContextOptionsBuilder<GeipanContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new GeipanContext(options);

            // Initialisation des données factices
            var type1 = new PAN.context.Models.Type { IdType = 1, Nom = "OVNI" };
            var loc1 = new Localisation { IdLocalisation = 1, Ville = "Paris", CodePostal = 75000 };

            var ev1 = new Evenement
            {
                IdEvenement = 1,
                DateHeureObservation = new DateTime(2023, 1, 1),
                Descriptif = "Lumière étrange",
                Estmouvant = true,
                UpVote = 5,
                Latitude = 48.8566m,
                Longitude = 2.3522m,
                IdType = 1,
                IdTypeNavigation = type1,
                IdLocalisation = 1,
                IdLocalisationNavigation = loc1
            };

            var ev2 = new Evenement
            {
                IdEvenement = 2,
                DateHeureObservation = new DateTime(2023, 1, 2), // Plus récent
                Descriptif = "Objet triangulaire",
                Estmouvant = false,
                UpVote = 2,
                Latitude = 45.0m,
                Longitude = 4.0m,
                IdType = 1,
                IdTypeNavigation = type1,
                IdLocalisation = 1,
                IdLocalisationNavigation = loc1
            };

            context.Type.Add(type1);
            context.Localisation.Add(loc1);
            context.Evenement.AddRange(ev1, ev2);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEventsOrderedByDateDescending()
        {
            // Arrange
            using var context = await GetInMemoryContextAsync(Guid.NewGuid().ToString());
            var service = new EvenementService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            // Vérifie l'ordre décroissant (l'événement 2 est plus récent que le 1)
            Assert.Equal(2, result[0].IdEvenement);
            Assert.Equal(1, result[1].IdEvenement);
        }

        [Fact]
        public async Task SearchPagedAsync_WithTextFilter_ReturnsFilteredEvents()
        {
            // Arrange
            using var context = await GetInMemoryContextAsync(Guid.NewGuid().ToString());
            var service = new EvenementService(context);

            // Act - Recherche du terme "triangulaire"
            var result = await service.SearchPagedAsync("triangulaire", null, null, null, 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result.First().IdEvenement);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEventDetail()
        {
            // Arrange
            using var context = await GetInMemoryContextAsync(Guid.NewGuid().ToString());
            var service = new EvenementService(context);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Paris", result.Ville);
            Assert.Equal("OVNI", result.TypeNom);
            Assert.Equal("Lumière étrange", result.Descriptif);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var context = await GetInMemoryContextAsync(Guid.NewGuid().ToString());
            var service = new EvenementService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddUpVoteAsync_ExistingEvent_IncrementsUpVoteAndReturnsTrue()
        {
            // Arrange
            using var context = await GetInMemoryContextAsync(Guid.NewGuid().ToString());
            var service = new EvenementService(context);

            // Act
            var success = await service.AddUpVoteAsync(1);
            var updatedEvent = await context.Evenement.FindAsync(1);

            // Assert
            Assert.True(success);
            Assert.Equal(6, updatedEvent.UpVote); // 5 (initial) + 1
        }

        [Fact]
        public async Task AddUpVoteAsync_NonExistingEvent_ReturnsFalse()
        {
            // Arrange
            using var context = await GetInMemoryContextAsync(Guid.NewGuid().ToString());
            var service = new EvenementService(context);

            // Act
            var success = await service.AddUpVoteAsync(999);

            // Assert
            Assert.False(success);
        }
    }
}
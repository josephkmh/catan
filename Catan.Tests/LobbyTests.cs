using System;
using System.Collections.Generic;
using Xunit;
using Catan.App;

namespace Catan.Tests
{
    public class LobbyTests
    {
        [Fact]
        public void TestAddGame()
        {
            Lobby lobby = new Lobby();
            Game game = new Game();
            
            lobby.Games.Add(game);

            List<Game> expectedGames = new List<Game>() {game};
            Assert.Equal(expectedGames, lobby.Games);
        }

        [Fact]
        public void TestAddConnectedUser()
        {
            Lobby lobby = new Lobby();
            User newUser1 = new User()
            {
                Id = "testUserId_1",
                Username = "testUsername_1"
            };
            User newUser2 = new User()
            {
                Id = "testUserId_1",
                Username = "testUsername_1"
            };

            lobby.AddConnectedUser("connectionId_1", newUser1);
            lobby.AddConnectedUser("connectionId_2", newUser2);

            Assert.Equal(2, lobby.ConnectedUsers.Count);
        }

        [Fact]
        public void TestRemoveConnectedUser()
        {
            Lobby lobby = new Lobby();
            User newUser1 = new User()
            {
                Id = "testUserId_1",
                Username = "testUsername_1"
            };
            User newUser2 = new User()
            {
                Id = "testUserId_1",
                Username = "testUsername_1"
            };

            lobby.AddConnectedUser("connectionId_1", newUser1);
            lobby.AddConnectedUser("connectionId_2", newUser2);
            lobby.RemoveConnectedUser("connectionId_1");

            Assert.Single(lobby.ConnectedUsers);
        }
    }
}
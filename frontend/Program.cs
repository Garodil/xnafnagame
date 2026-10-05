using System;
using frontend.Game;
using Microsoft.Xna.Framework;
using model.Interfaces.Factory;
using model.Interfaces.Service;
using service;
using service.Logic;
using model.Interfaces.Repository;
using repository;
using Microsoft.Xna.Framework.Content;
using logic.Component;
using scenes;
using service.Config;
using model.Models.Configs;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var optionsManager = new ConfigManagerService();
        var options = optionsManager.JsonDeserialize();
        using var g = new Backbone(options, optionsManager);
        g.Run();
    }
}
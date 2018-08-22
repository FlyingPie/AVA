﻿using AVA.Plugins.Dummy;
using MUI;
using MUI.DI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AVA
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            SetupNativeDependencies();

            // TODO: Make nicer
            typeof(DummyQueryExecutor).GetType();
            typeof(Indexing.Indexer).GetType();
            typeof(Plugins.FirefoxBookmarks.FirefoxBookmarksQueryExecutor).GetType();
            typeof(Plugins.Time.TimeQueryExecutor).GetType();

            var uiContext = new UIContext(600, 300);

            var container = new Container()
                .Register<ResourceManager, ResourceManager>(c => uiContext.ResourceManager)
                .Register(uiContext)
                .Register<UI, UI>()
            ;

            RegisterServices(container);

            // TODO: Remove (pries cache)
            container.Resolve<Indexing.Indexer>().Query("conemu");

            uiContext.PushUI(container.Resolve<UI>());
            uiContext.Run();
        }

        private static void SetupNativeDependencies()
        {
            var output = Path.GetDirectoryName(new Uri(typeof(Program).Assembly.CodeBase).LocalPath);

            foreach (var dependency in Directory.GetFiles(GetDependenciesDirectory()))
            {
                var fileName = Path.GetFileName(dependency);
                var target = Path.Combine(output, fileName);
                if (!File.Exists(target))
                {
                    File.Copy(dependency, target);
                }
            }
        }

        private static string GetDependenciesDirectory()
        {
            var root = "Deps/";

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return Path.Combine(root, "win64");
            }

            throw new NotSupportedException($"Apparently, platform is not supported: '{Environment.OSVersion}'");
        }

        private static void RegisterServices(IContainer container) => Assembly
            .GetEntryAssembly()
            .GetReferencedAssemblies()
            .Select(ass => Assembly.Load(ass))
            .SelectMany(ass => ass.DefinedTypes)
            .Where(t => !t.IsAbstract)
            .Where(t => t.GetCustomAttribute<ServiceAttribute>(true) != null)
            .ToList()
            .ForEach(s => container.Register(s.AsType(), s.GetCustomAttribute<ServiceAttribute>(true).Lifetime));
    }
}
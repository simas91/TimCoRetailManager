﻿using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;
using TRMDesktopUI.ViewModels;

namespace TRMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        // AutoMapper
        private IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Maps from ProductModel to ProductDisplayModel
                // Maps from CartItemModel to CartItemDisplayModel
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });
            var output = config.CreateMapper();

            return output;
        }

        protected override void Configure()
        {
            // puts AutoMapper instance in container
            _container.Instance(ConfigureAutoMapper());

            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>()
                .PerRequest<ISaleEndPoint, SaleEndPoint>();
            

            // brings extra stuff from caliburn.micro
            // SINGLETON
            // makes that class to be used for entire duration of the app, instead of making new one on request
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<IAPIHelper, APIHelper>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}

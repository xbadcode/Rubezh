﻿using Infrastructure;
using Moq;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using SKDModule.ViewModels;
using SKDModuleTest.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKDModuleTest
{
	[TestFixture]
	public class OrganisationsTest
	{
		MockDialogService MockDialogService;
		MockMessageBoxService MockMessageBoxService;
		User User;

		[SetUp]
		public void Initialize()
		{
			ClientManager.SecurityConfiguration = new SecurityConfiguration();
			User = new User()
			{
				Login = "adm"
			};
			ClientManager.SecurityConfiguration.Users.Add(User);
			ClientManager._userLogin = "adm";

			ServiceFactory.Initialize(null, null);
			ServiceFactory.DialogService = MockDialogService = new MockDialogService();
			ServiceFactory.MessageBoxService = MockMessageBoxService = new MockMessageBoxService();
		}

		[Test]
		public void ViewOrganisationTest()
		{
			var mock = new Mock<ISafeFiresecService>();
			mock.Setup(x => x.GetOrganisations(It.IsAny<OrganisationFilter>())).Returns(() => new OperationResult<List<Organisation>>() { Result = new List<Organisation>() { new Organisation() { Name = "Test" } } });
			ClientManager.FiresecService = mock.Object;

			var organisationsViewModel = new OrganisationsViewModel();
			organisationsViewModel.Initialize(LogicalDeletationType.All);
			Assert.IsTrue(organisationsViewModel.Organisations.Count == 1);
			Assert.IsTrue(organisationsViewModel.SelectedOrganisation.Organisation.Name == "Test");
			Assert.IsTrue(organisationsViewModel.OrganisationDoorsViewModel.Items.Count == 0);
			Assert.IsTrue(organisationsViewModel.OrganisationUsersViewModel.Items.Count == 1);
			Assert.IsTrue(organisationsViewModel.OrganisationUsersViewModel.Items[0].IsChecked == false);
		}

		[Test]
		public void AddOrganisationTest()
		{
			User.PermissionStrings.Add("Oper_SKD_Organisations_AddRemove");
			MockDialogService.OnShowModal += x =>
			{
				var organisationDetailsViewModel = x as OrganisationDetailsViewModel;
				organisationDetailsViewModel.Name = "Test";
				organisationDetailsViewModel.SaveCommand.Execute();

			};

			var mock = new Mock<ISafeFiresecService>();
			mock.Setup(x => x.GetOrganisations(It.IsAny<OrganisationFilter>())).Returns(() => new OperationResult<List<Organisation>>() { Result = new List<Organisation>() });
			mock.Setup(x => x.GetEmployeeList(It.IsAny<EmployeeFilter>())).Returns(() => new OperationResult<List<ShortEmployee>>() { Result = new List<ShortEmployee>() });
			mock.Setup(x => x.SaveOrganisation(It.IsAny<OrganisationDetails>(), It.IsAny<bool>())).Returns(() => new OperationResult<bool>() { Result = true });
			ClientManager.FiresecService = mock.Object;

			var organisationsViewModel = new OrganisationsViewModel();
			organisationsViewModel.Initialize(LogicalDeletationType.All);
			organisationsViewModel.AddCommand.Execute();
			Assert.IsTrue(organisationsViewModel.Organisations.Count == 1);
			Assert.IsTrue(organisationsViewModel.SelectedOrganisation.Organisation.Name == "Test");
		}
	}
}
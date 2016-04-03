﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeCardsCtrl',
        ['$scope', '$uibModal', '$window', 'employeesService',
         function ($scope, $uibModal, $window, employeesService) {
             $scope.selectedCard = null;

             $scope.photoData = { photo: null };

             $scope.gridOptionsDoors = {
                 enableRowHeaderSelection: false,
                 enableSorting: false,
                 multiSelect: false,
                 enableColumnMenus: false,
                 enableRowSelection: true,
                 noUnselect: true,
                 columnDefs: [
                     { field: 'PresentationName', width: 210, displayName: 'Точка доступа' },
                     { field: 'EnerScheduleName', width: 210, displayName: 'Вход' },
                     { field: 'ExitScheduleName', width: 210, displayName: 'Выход' }
                 ]
             };

             $scope.$watch(function () {
                 return employeesService.selectedEmployee;
             }, function(employee) {
                 $scope.selectedEmployee = employee;
                 $scope.selectedCard = null;
                 $scope.isGuest = (employee.Model.Type === 1);
                 employeesService.getEmployeeCards($scope.selectedEmployee.UID).then(function(cards) {
                     $scope.employeeCards = cards;
                     return employeesService.getEmployeePhoto($scope.selectedEmployee.UID);
                 }).then(function(photo) {
                     $scope.photoData.photo = photo || "//:0";
                 });
             });

             $scope.cardClick = function(card) {
                 $scope.selectedCard = employeesService.selectedCard = card;
                 if (card) {
                     $scope.gridOptionsDoors.data = card.Doors;
                 }
             };

             $scope.gridStyleDoors = function () {
                 var ctrlHeight = ($window.innerHeight - 100) / 2;
                 return "height:" + ctrlHeight + "px";
             }();
         }]
    );

}());

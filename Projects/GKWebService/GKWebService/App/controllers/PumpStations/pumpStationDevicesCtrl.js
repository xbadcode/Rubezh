﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('pumpStationDevicesCtrl',
        ['$scope', '$http', '$uibModal', 'signalrDevicesService', 'dialogService', 'constants',
        function ($scope, $http, $uibModal, signalrDevicesService, dialogService, constants) {

            $scope.uiGrid = {
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: false,
                modifierKeysToMultiSelect: true,
                noUnselect: true,
                enableSorting: false,
                enableColumnResizing: true,
                enableColumnMenus: false,
                columnDefs:
                  [{ field: 'Name', displayName: 'Насосы', width: 300, cellTemplate: '<div class="ui-grid-cell-contents" style="float:left" ><a href="" ng-click="grid.appScope.pumpClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> <img style="vertical-align: middle" width="16px" height="16px" ng-src="/Content/Image/{{row.entity.ImageSource}}" /> {{row.entity[col.field]}}</a></div>' },
                   { field: 'Address', displayName: 'Адрес', width: 150 },
                   { field: 'NsLogic', displayName: 'Дополнительное условие включения', enableColumnResizing: false, minWidth: 400 }]
                 };

            $scope.gridStyle = function () {
            	var ctrlHeight = window.innerHeight - 200;
            	return "height:" + ctrlHeight + "px";
            }();

            $scope.pumpClick = function (device) {
              
                dialogService.showWindow(constants.gkObject.device, device);
            };

            function ChangeDevices(device) {
                for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                    if ($scope.uiGrid.data[i].UID == device.UID) {
                        $scope.uiGrid.data[i] = device;
                        break;
                    }
                }
            };

            signalrDevicesService.onDeviceChanged( function (event, args) {
                ChangeDevices(args);
                $scope.$apply();
            });


            $scope.$on('pumpStationDevicesChanged', function (event, args) {
                $http.get('PumpStations/GetPumpStationDevices/' + args
              ).success(function (data, status, headers, config) {
                  $scope.uiGrid.data = data;
              });
            });

        }]);
}());
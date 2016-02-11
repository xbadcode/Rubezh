﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('pumpStationDevicesCtrl',
        ['$scope', '$http', '$uibModal', 'signalrDevicesService',
        function ($scope, $http, $uibModal, signalrDevicesService) {

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
                  [{ field: 'Name', displayName: 'Насосы', width: 300, cellTemplate: '<div class="ui-grid-cell-contents" style="float:left" ><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> <img style="vertical-align: middle" width="16px" height="16px" ng-src="/Content/Image/{{row.entity.ImageSource}}" /> {{row.entity[col.field]}}</a></div>' },
                   { field: 'Address', displayName: 'Адрес', width: 100 },
                   { field: 'NsLogic', displayName: 'Дополнительное условие включения', width: 400 }]
                 };

            $scope.mptClick = function (mpt) {
                $uibModal.open({
                    animation: false,
                    templateUrl: 'MPTs/MPTDetails',
                    controller: 'mptsDetailsCtrl',
                    resolve: {
                        mpt: function () {
                            return mpt;
                        }
                    }
                });
            };

            function ChangeDevices(device) {
                for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                    if ($scope.uiGrid.data[i].UID == device.UID) {
                        $scope.uiGrid.data[i] = device;
                        break;
                    }
                }
            };

            $scope.$on('devicesChanged', function (event, args) {
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
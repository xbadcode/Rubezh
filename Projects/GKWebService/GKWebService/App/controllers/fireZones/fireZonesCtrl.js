﻿(function () {

	'use strict';

	var app = angular.module('gkApp.controllers').controller('fireZonesCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'uiGridConstants', 'signalrFireZonesService', 'broadcastService', 'dialogService', 'constants',
        function ($scope, $http, $timeout, $uibModal, $stateParams, uiGridConstants, signalrFireZonesService, broadcastService, dialogService, constants) {
        	var fireCountWidth = Math.round(($(window).width() - 525) / 2);
        	$scope.gridOptions = {
        		enableRowHeaderSelection: false,
        		enableSorting: false,
        		multiSelect: false,
        		enableColumnMenus: false,
        		enableRowSelection: true,
        		noUnselect: true,
        		enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
        		columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/{{row.entity.ImageSource}}"/>{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 210, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="" ng-click="grid.appScope.fireZonesClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'Fire1Count', displayName: 'Количество датчиков для перевода в Пожар1:', width: fireCountWidth },
                    { field: 'Fire2Count', displayName: 'Количество датчиков для перевода в Пожар2:', width: fireCountWidth }
        		]
        	};

        	$scope.gridStyle = function () {
        		var ctrlHeight = (window.innerHeight - 100)/2;
        		return "height:" + ctrlHeight + "px";
        	}();

        	$scope.$on('fireZonesChanged', function (event, args) {
        		for (var i in $scope.gridOptions.data) {
        		    if (args.UID === $scope.gridOptions.data[i].UID) {
        				$scope.gridOptions.data[i].CanResetIgnore = args.CanResetIgnore;
        				$scope.gridOptions.data[i].CanSetIgnore = args.CanSetIgnore;
        				$scope.gridOptions.data[i].CanResetFire = args.CanResetFire;
        				$scope.gridOptions.data[i].StateColor = args.StateColor;
        				$scope.gridOptions.data[i].StateIcon = args.StateIcon;
        				$scope.gridOptions.data[i].StateMessage = args.StateMessage;
        				$scope.$apply();
        				break;
        			}
        		}
        	});

        	$scope.fireZonesClick = function (fireZone) {
        	    dialogService.showWindow(constants.gkObject.zone, fireZone);
        	};

        	$scope.changeZone = function (row) {
        	    broadcastService.send('selectedZoneChanged', row.entity.UID);
        	}

        	$http.get('FireZones/GetFireZonesData').success(function (data) {
        		$scope.gridOptions.data = data;
        		//Выбираем первую строку после загрузки данных
        		$timeout(function () {
        			if ($stateParams.uid) {
        				$scope.selectRowById($stateParams.uid);
        			} else {
        				if ($scope.gridApi.selection.selectRow) {
        					$scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
        				}
        			}
        			$scope.gridApi.autoSize.fit($scope.gridOptions.columnDefs[0]);
        		});
        	});

        	$scope.gridOptions.onRegisterApi = function (gridApi) {
        		$scope.gridApi = gridApi;


        		gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
        			$scope.gridApi.selection.selectRow(newRowCol.row.entity);
        		});


        		//Подписчик на событие изменения выбранной строки
        		gridApi.selection.on.rowSelectionChanged($scope, $scope.changeZone);
        	};



        	$scope.selectRowById = function (uid) {
        		for (var i = 0; i < $scope.gridOptions.data.length; i++) {
        		    if ($scope.gridOptions.data[i].UID === uid) {
        				$scope.gridApi.selection.selectRow($scope.gridOptions.data[i]);
        				$scope.gridApi.core.scrollTo($scope.gridOptions.data[i], $scope.gridOptions.columnDefs[0]);
			            break;
			        }
        		}
        	}
        }]
    );
}());

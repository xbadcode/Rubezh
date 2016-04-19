﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('cardsCtrl',
        ['$scope', '$timeout', '$window', 'cardsService', 
         function ($scope, $timeout, $window, cardsService) {
            $scope.gridOptions = {
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, function(row) {
                        cardsService.selectedCard = row.entity;
                    });
                },                
                enableRowHeaderSelection: false,
                enableSorting: false,
                showTreeExpandNoChildren: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                showTreeRowHeader: false,
                columnDefs: [
                    { field: 'Number', width: 210, displayName: 'Номер', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><img style='vertical-align: middle; padding-right: 3px' width='16' height='16' ng-show='!row.entity.IsOrganisation && row.entity.IsDeactivatedRootItem' ng-src='/Content/Image/Icon/Hr/Lock.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation && !row.entity.IsDeactivatedRootItem' ng-src='/Content/Image/Icon/Hr/Card.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'CardType', width: 210, displayName: 'Тип' },
                    { field: 'EmployeeName', width: 210, displayName: 'Сотрудник' },
                    { field: 'StopReason', width: 210, displayName: 'Причина деактивации' }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100);
                return "height:" + ctrlHeight + "px";
            }();

            var reloadTree = function () {
                cardsService.getCards($scope.filter)
                    .then(function (cards) {
                        $scope.cards = cards;
                        $scope.gridOptions.data = $scope.cards;
                        cardsService.selectedCard = null;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                    });
                });
            }

            cardsService.reload = reloadTree;

            $scope.$watch('filter', function (newValue, oldValue) {
                reloadTree();
            }, true);

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };
         }]
    );

}());
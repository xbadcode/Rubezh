System.register(['@angular/core', '@angular/router', '@angular/http', './plans-list.component', './plan.component'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, router_1, http_1, plans_list_component_1, plan_component_1;
    var PlansComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (plans_list_component_1_1) {
                plans_list_component_1 = plans_list_component_1_1;
            },
            function (plan_component_1_1) {
                plan_component_1 = plan_component_1_1;
            }],
        execute: function() {
            PlansComponent = (function () {
                function PlansComponent(_router) {
                    this._router = _router;
                }
                PlansComponent.prototype.ngOnInit = function () {
                };
                PlansComponent = __decorate([
                    core_1.Component({
                        selector: '[gk-plans]',
                        templateUrl: 'app/components/+plans/plans.component.html',
                        directives: [router_1.ROUTER_DIRECTIVES, plans_list_component_1.PlansListComponent],
                        providers: [http_1.HTTP_PROVIDERS]
                    }),
                    router_1.Routes([
                        { path: '/:id', component: plan_component_1.PlanComponent }
                    ]), 
                    __metadata('design:paramtypes', [router_1.Router])
                ], PlansComponent);
                return PlansComponent;
            }());
            exports_1("PlansComponent", PlansComponent);
        }
    }
});
//# sourceMappingURL=plans.component.js.map
System.register(['./plans.component', './plans-list.component', './+models/planInfo.model', './+models/planElement.model'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    function exportStar_1(m) {
        var exports = {};
        for(var n in m) {
            if (n !== "default") exports[n] = m[n];
        }
        exports_1(exports);
    }
    return {
        setters:[
            function (plans_component_1_1) {
                exportStar_1(plans_component_1_1);
            },
            function (plans_list_component_1_1) {
                exportStar_1(plans_list_component_1_1);
            },
            function (planInfo_model_1_1) {
                exportStar_1(planInfo_model_1_1);
            },
            function (planElement_model_1_1) {
                exportStar_1(planElement_model_1_1);
            }],
        execute: function() {
        }
    }
});
//# sourceMappingURL=index.js.map
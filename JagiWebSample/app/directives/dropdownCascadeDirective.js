(function () {
    'use strict';
    
    window.app.directive('dropdownCascade', dropdownCascade);

    function dropdownCascade($compile) {
        var code;
        var options = [];
        return {
            restrict: "A",
            scope: {
                dropdownCascade: '=',   // 取出 parent code
                cascadeOptions: '='     // 傳回去 child code options
            },
            link: function (scope, elem, attrs) {
                code = attrs.name;
            },
            controller: ['$scope', 'dataService', function ($scope, dataService) {
                $scope.$watch(function () { return $scope.dropdownCascade; }, function (newValue, oldValue) {
                    if (newValue && (newValue != oldValue)) {
                        dataService.getCodeDetails(code, $scope.dropdownCascade,
                            function (response) {
                                $scope.cascadeOptions = response.data;
                            });
                    }
                });
                //if ($scope.dropdownCascade) {
                //    dataService.getCodeDetails($scope.dropdownCascade, function (response) {
                //        options = response.data;
                //    });
                //}
            }],
            //compile: function compile(elem, attrs) {
            //    code = attrs.name;
            //    element = elem;
            //    return {
            //        pre: function preLink(scope, elem, attrs) {
            //            if (options.length > 0) {
            //                for (var i = 0; i < options.length; i++) {
            //                    elem.append("<option value='{0}'>{1}</option>"
            //                        .format(options[i].itemCode, options[i].description));
            //                }
            //            }
            //        },
            //    }
            //}
        };
    }

    //function dropdownCascade($parse) {
    //    return {
    //        scope: {
    //            parentValue: '=',
    //            value: '=currentValue'
    //        },
    //        link: function(scope, elem, attrs){
    //            var test = scope.parentValue;
    //            var test2 = scope.value;
    //            var test3 = $parse(attrs.parentValue)(scope);

    //        },
    //        controller: controller,
    //    }
    //}

    //controller.$inject = ['$scope', 'dataService'];
    //function controller($scope, dataService) {

    //    $scope.$watch(function () { return $scope.value; }, function (newValue, oldValue) {
    //        console.log("into watch!");
    //    });

    //    var test = $scope.ngModel;
    //    if ($scope.dropdownCascade) {
    //        dataService.getCodeDetails($scope.dropdownCascade, function (response) {
    //            options = response.data;
    //        });
    //    }
    //}
})();
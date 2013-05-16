function LicenseListCtrl($scope, $location, $routeParams, License) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };

    $scope.entityId = '';
    $scope.entity = '';
    $scope.licenseUrl = '#!/licenses';

    if ($routeParams.productId) {
        $scope.entityId = $routeParams.productId;
        $scope.entity = 'products';
        $scope.licenseUrl = '#!/' + $scope.entity + '/' + $scope.entityId + '/licenses';
    }
    else if ($routeParams.customerId) {
        $scope.entityId = $routeParams.customerId;
        $scope.entity = 'customers';
        $scope.licenseUrl = '#!/' + $scope.entity + '/' + $scope.entityId + '/licenses';
    }

    $scope.licenses = License.query({ findByEntity: $scope.entity, entityId: $scope.entityId },
        function (success, getResponseHeaders) {
            $scope.notificationAlert.show = false;
        },
        function (error, getResponseHeaders) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = error.data.responseStatus.message;
        });

    $scope.deleteLicense = function (license) {
        License.delete({ id: license.id },
            function (success, getResponseHeaders) {
                $scope.licenses.splice($scope.licenses.indexOf(license), 1);
                $scope.notificationAlert.show = false;
            },
            function (error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };
}

//LicenseListCtrl.$inject = ['$scope', '$location', '$routeParams', 'License'];


function LicenseAddCtrl($scope, $location, $routeParams, $log, $http, License, Customer, Product) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };
    $scope.emptyModel = {};
    $scope.license = angular.copy($scope.emptyModel);

    $scope.entityId = '';
    $scope.entity = '';

    if ($routeParams.productId) {
        $scope.entityId = $routeParams.productId;
        $scope.entity = 'products';

        $scope.license.productId = $scope.entityId;
        $scope.customers = Customer.query();

    } else if ($routeParams.customerId) {
        $scope.entityId = $routeParams.customerId;
        $scope.entity = 'customers';

        $scope.license.customerId = $scope.entityId;
        $scope.products = Product.query();

    } else {
        $scope.products = Product.query();
        $scope.customers = Customer.query();
    }

    $http({ method: 'GET', url: 'api/licenses/types' }).
        success(function (data, status, headers, config) {
            $scope.licenseTypes = data;
        }).
        error(function (data, status, headers, config) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = data.responseStatus.message;
        });

    $scope.addLicense = function (newLicense) {
        var lic = new License(newLicense);

        lic.$save({},
            function (success, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'success';
                $scope.notificationAlert.message = 'Successfuly created!';

                //$location.path('/licenses');
            },
            function (error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };

    $scope.cancel = function () {
        $scope.license = angular.copy($scope.emptyModel);
    };
}

//LicenseAddCtrl.$inject = ['$scope', '$location', 'License'];
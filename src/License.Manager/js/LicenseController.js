function LicenseListCtrl($scope, $location, $routeParams, $window, $log, License) {

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

    $scope.issueLicense = function (license) {
        License.issue({ id: license.id },
            function (success, getResponseHeaders) {
                $scope.notificationAlert.show = false;
                $window.open(getResponseHeaders('location'));
            },
            function (error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };

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
    $scope.emptyModel = {
        productId: '',
        customerId: '',
        productFeatures: [],
        additionalAttributes: []
    };
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

    $scope.addProductFeature = function () {
        $scope.license.productFeatures.push({ "Key": "", "Value": "Product Feature Value" });
    };

    $scope.removeProductFeature = function (index) {
        $scope.license.productFeatures.splice(index, 1);
    };

    $scope.addAdditionalAttribute = function () {
        $scope.license.additionalAttributes.push({ "Key": "New Attribute", "Value": "New Attribute Value" });
    };

    $scope.removeAdditionalAttribute = function (index) {
        $scope.license.additionalAttributes.splice(index, 1);
    };

    $scope.$watch('license.productId', function (newValue, oldValue) {
        
        if (angular.isUndefined(newValue) || newValue < 1)
            return;
        
        $scope.product = Product.get({ id: newValue },
            function (success, getResponseHeaders) {

                $scope.license.productFeatures.length = 0; //empty array with creating a new one

                angular.forEach(success.productFeatures, function (val, key) {
                    $scope.license.productFeatures.push({ "Key": val, "Value": "" });
                });
            },
            function (error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    });
    
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
        lic.productFeatures = toDictionary(newLicense.productFeatures);
        lic.additionalAttributes = toDictionary(newLicense.additionalAttributes);

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
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
        function(success, getResponseHeaders) {
            $scope.notificationAlert.show = false;
        },
        function(error, getResponseHeaders) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = error.data.responseStatus.message;
        });
    
    $scope.deleteLicense = function (license) {
        License.delete({ id: license.id },
            function(success, getResponseHeaders) {
                $scope.licenses.splice($scope.licenses.indexOf(license), 1);
                $scope.notificationAlert.show = false;
            },
            function(error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };
}

//LicenseListCtrl.$inject = ['$scope', '$location', '$routeParams', 'License'];

